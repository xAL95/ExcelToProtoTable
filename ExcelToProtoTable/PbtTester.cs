using ProtoBuf;
using ProtoTable;
using System;
using System.IO;

namespace ExcelToProtoTable
{
    internal static class PbtTester
    {
        internal static void TestPbt(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine($"<TEST>Could not find file: {filePath}");
                return;
            }

            var protoTable = LoadPbt<BaseProtoTable>(filePath);
            if (protoTable == null)
            {
                return;
            }

            Console.WriteLine($"<TEST>PBR File: {protoTable.GetType()} has {protoTable.Datas.Count} entries");
        }

        private static T? LoadPbt<T>(string filePath) where T : class
        {
            var bytes = LoadFile(filePath);
            if (bytes == null)
            {
                return null;
            }

            try
            {
                using var source = new MemoryStream(bytes);
                return Serializer.Deserialize<T>(source);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        private static byte[]? LoadFile(string strPath)
        {
            try
            {
                if (string.IsNullOrEmpty(strPath) || !File.Exists(strPath))
                {
                    return null;
                }

                return File.ReadAllBytes(strPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}
