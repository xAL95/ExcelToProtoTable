using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ProtoBuf;
using ProtoTable;

namespace ExcelToProtoTable
{
	internal class Program
	{
		const string tableFileExt = ".pbt";

		static async Task Main(string[] args)
		{
			if(args.Length == 0) return;

            Console.WriteLine("Please wait ...");

            var sw = Stopwatch.StartNew();

            // create async task
            List<Task> tasks = new List<Task>();

            foreach (var arg in args)
            {
                var task = Task.Run(() => Job(arg));

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            sw.Stop();

            Console.WriteLine($"Tasks Finished. Time required in MS: {sw.ElapsedMilliseconds}");
        }

        private static void Job(string arg)
        {
            if (string.IsNullOrEmpty(arg)) return;

            var fullPath = Path.GetFullPath(arg);
            if (string.IsNullOrEmpty(fullPath))
                return;

            var directoryName = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrEmpty(directoryName))
                return;

            var fileExt = Path.GetExtension(fullPath);
            if (string.IsNullOrEmpty(fileExt))
                return;

            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fullPath);
            if (string.IsNullOrEmpty(fileNameWithoutExt))
                return;

            string exportPath = Path.Combine(directoryName, tableFileExt);
            string fullExportPath = Path.Combine(exportPath, fileNameWithoutExt + tableFileExt);

            //Console.WriteLine($"Full Path: {fullPath}\nDirectory Name: {directoryName}\nFile Name: {fileNameWithoutExt}, File Ext: {fileExt}\nExport Path: {exportPath}");

            var baseProtoTable = CreateTableInstance(fileNameWithoutExt) as BaseProtoTable;
            if (baseProtoTable == null)
            {
                Console.WriteLine($"Table class '{fileNameWithoutExt}' not a valid ProtoTable.");

                return;
            }

            string tableTypeName = baseProtoTable.GetType().Name;
            string tableDataClassName = $"{tableTypeName}+d_{tableTypeName}";

            //Console.WriteLine($"tableTypeName: '{tableTypeName}', tableDataClassName: '{tableDataClassName}'");

            // process excel file
            ProcessExcelFile(fullPath, baseProtoTable, tableDataClassName);

            // create export directory
            Directory.CreateDirectory(exportPath);

            // save result
            SaveDataToFile(fullExportPath, baseProtoTable);

            // Test file
            PbtTester.TestPbt(fullExportPath);
        }

        private static void ProcessExcelFile(string fullPath, BaseProtoTable baseProtoTable, string tableDataClassName)
        {
            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);

                ISheet sheet = xssWorkbook.GetSheetAt(0);
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    var baseProtoTableData = CreateTableInstance(tableDataClassName) as BaseProtoTableData;
                    if (baseProtoTableData == null)
                    {
                        Console.WriteLine($"Nested Table Data class '{tableDataClassName}' is not a valid IExtensible.");
                        return;
                    }

                    IRow row = sheet.GetRow(i);
                    if (row == null || row.Cells.All(d => d.CellType == CellType.Blank))
                        continue;

                    SetRowData(baseProtoTableData, row);
                    baseProtoTable.Datas.Add(baseProtoTableData);
                }
            }
        }
        private static void SetRowData(BaseProtoTableData baseProtoTableData, IRow row)
        {
            var props = baseProtoTableData.GetType().GetProperties();
            for (int j = 0; j < props.Length; j++)
            {
                var value = GetValue(props[j], row.GetCell(j));
                props[j].SetValue(baseProtoTableData, value);
            }
        }
        private static object GetValue(PropertyInfo propInfo, ICell cell)
        {
            switch (propInfo.PropertyType.Name.ToLower())
            {
                case "string":
                    return cell == null ? string.Empty : cell.StringCellValue;
                case "uint32":
                    return uint.TryParse(cell.ToString(), out uint res) ? res : 0u;
                case "uint64":
                    return ulong.TryParse(cell.ToString(), out ulong re) ? re : 0uL;
                case "float":
                    return float.TryParse(cell.ToString(), out float resu) ? resu : 0f;
                case "single":
                    return float.TryParse(cell.ToString(), out float single) ? single : 0f;
                case "double":
                    return double.TryParse(cell.ToString(), out double resul) ? resul : 0d;
                case "boolean":
                    return bool.TryParse(cell.ToString(), out var result) ? result : false;

                default: Console.WriteLine("Unknown PropertyType: " + propInfo.PropertyType.Name.ToLower()); break;
            }

            return string.Empty;
        }
        private static void SaveDataToFile(string exportPath, BaseProtoTable baseProtoTable)
        {
            using (var fs = File.Create(exportPath))
            {
                if (SaveProtoBuffData(fs, baseProtoTable))
                {
                    Console.WriteLine($"Generated File {exportPath} with {baseProtoTable.Datas.Count} table entries successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to save data to {exportPath}");
                }
            }
        }
        private static bool SaveProtoBuffData<T>(Stream destination, T instance) where T : class
        {
            try
            {
                Serializer.Serialize(destination, instance);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }
        private static object? CreateTableInstance(string name)
		{
			string className = "ProtoTable.Model." + name;

			var type = Type.GetType(className);
			if (type == null)
			{
				return null;
			}

			return Activator.CreateInstance(type);
		}


	}
}
