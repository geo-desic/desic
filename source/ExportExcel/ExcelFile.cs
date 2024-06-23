using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Data.Common;

namespace ExportExcel
{
    internal class ExcelFile(string filePath)
    {
        private string _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        private SharedStringTablePart? _sharedStringTablePart;

        public async Task Create(DbDataReader reader, string?[]? worksheetNames = null)
        {
            // https://learn.microsoft.com/en-us/office/open-xml/spreadsheet/working-with-sheets?tabs=cs
            using var document = SpreadsheetDocument.Create(_filePath, SpreadsheetDocumentType.Workbook);
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());

            _sharedStringTablePart = null;
            if (workbookPart.GetPartsOfType<SharedStringTablePart>().Any())
            {
                _sharedStringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
            }
            else
            {
                _sharedStringTablePart = workbookPart.AddNewPart<SharedStringTablePart>();
            }

            var nextResult = true;
            var resultSetIndex = 0u;
            while (nextResult)
            {
                ++resultSetIndex;
                var schemaTable = reader.GetSchemaTable()!;

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                var worksheet = new Worksheet(sheetData);
                worksheetPart.Worksheet = worksheet;

                var sheet = new Sheet()
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = resultSetIndex,
                    Name = worksheetNames?.Length >= resultSetIndex ? worksheetNames[resultSetIndex - 1] : $"sheet{resultSetIndex}",
                };
                sheets.Append(sheet);

                var rowIndex = 0u;
                while (reader.Read())
                {
                    Row row;
                    if (rowIndex == 0)
                    {
                        ++rowIndex;
                        row = CreateHeaderRow(schemaTable, rowIndex);
                        sheetData.Append(row);
                    }
                    ++rowIndex;
                    row = CreateRow(schemaTable, rowIndex, reader);
                    sheetData.Append(row);
                }
                worksheetPart.Worksheet.Save();
                nextResult = await reader.NextResultAsync();
            }
        }

        private Row CreateHeaderRow(DataTable schemaTable, uint rowIndex)
        {
            var result = new Row { RowIndex = rowIndex };
            var columnIndex = 0;
            foreach (DataRow row in schemaTable.Rows)
            {
                ++columnIndex;
                var cell = new Cell { CellReference = ColumnIndexToColumnName(columnIndex) + rowIndex };
                result.InsertAt(cell, columnIndex - 1);
                var sharedStringIndex = InsertSharedStringItem(row["ColumnName"].ToString() ?? string.Empty);
                cell.CellValue = new CellValue(sharedStringIndex.ToString());
                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
            }
            return result;
        }

        private Row CreateRow(DataTable schemaTable, uint rowIndex, IDataRecord reader)
        {
            var result = new Row { RowIndex = rowIndex };
            for (var columnIndex = 1; columnIndex <= schemaTable.Rows.Count; ++columnIndex)
            {
                var cell = new Cell { CellReference = ColumnIndexToColumnName(columnIndex) + rowIndex };
                result.InsertAt(cell, columnIndex - 1);
                if (IsNumeric(schemaTable.Rows[columnIndex - 1]["DataType"].ToString()))
                {
                    cell.CellValue = new CellValue(reader[columnIndex - 1].ToString()!);
                    cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                }
                else
                {
                    var sharedStringIndex = InsertSharedStringItem(reader[columnIndex - 1].ToString() ?? string.Empty);
                    cell.CellValue = new CellValue(sharedStringIndex.ToString());
                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                }
            }
            return result;
        }

        // https://learn.microsoft.com/en-us/office/open-xml/spreadsheet/how-to-insert-text-into-a-cell-in-a-spreadsheet
        private int InsertSharedStringItem(string text)
        {
            _sharedStringTablePart!.SharedStringTable ??= new SharedStringTable();

            var i = 0;
            foreach (SharedStringItem item in _sharedStringTablePart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }
                ++i;
            }

            _sharedStringTablePart.SharedStringTable.AppendChild(new SharedStringItem(new Text(text)));
            _sharedStringTablePart.SharedStringTable.Save();

            return i;
        }

        // https://learn.microsoft.com/en-us/office/troubleshoot/excel/convert-excel-column-numbers
        private static string ColumnIndexToColumnName(int columnIndex)
        {
            int a;
            int b;
            var result = "";
            while (columnIndex > 0)
            {
                a = (columnIndex - 1) / 26;
                b = (columnIndex - 1) % 26;
                result = (char)(b + 65) + result;
                columnIndex = a;
            }
            return result;
        }

        private static bool IsNumeric(string? dataTypeString)
        {
            return dataTypeString == "System.Byte"
                || dataTypeString == "System.Int16"
                || dataTypeString == "System.Int32"
                || dataTypeString == "System.Int64"
                || dataTypeString == "System.Int128"
                || dataTypeString == "System.UInt16"
                || dataTypeString == "System.UInt32"
                || dataTypeString == "System.UInt64"
                || dataTypeString == "System.UInt128";
        }
    }
}
