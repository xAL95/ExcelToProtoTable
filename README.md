# ExcelToProtoTable

ExcelToProtoTable is a lightweight tool written in .NET 8.0 that converts Excel files (.xlsx) into a compressed file format using Protocol Buffers (Protobuf). It’s designed to streamline the process of transforming Excel data into efficient and compact serialized objects.

## Requirements
To run ExcelToProtoTable, you will need:
- .NET 8.0
- NuGet packages:
  - protobuf-net
  - DotNetCore.NPOI

## How to Use
### Drag & Drop
You can easily convert Excel files by dragging and dropping them onto the ExcelToProtoTable.exe file.
- Example: Drag and drop Cars.xlsx or Food.xlsx onto the executable to instantly generate the corresponding Protobuf files.
  
You can also drag and drop multiple .xlsx files onto the executable, and the tool will convert them all at once.

### Command Line
Alternatively, you can use the command line to convert files:
```bash
ExcelToProtoTable.exe <file1.xlsx> <file2.xlsx> ...
```
This allows you to convert multiple Excel files at once.

## Sample Files
Check out the publish folder for example Excel files (Cars.xlsx, Food.xlsx) to test the tool's functionality.

## Example Usage:
```bash
ExcelToProtoTable.exe Cars.xlsx Food.xlsx
```

This will generate Protobuf files from the given Excel spreadsheets.
