using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Data.Common;

namespace ExportExcel
{
    internal class ExcelFile(string filePath)
    {
        private const uint DateNumberFormatId = 22; // m/d/yyyy H:mm => https://github.com/ClosedXML/ClosedXML/wiki/NumberFormatId-Lookup-Table
        private const uint DateStyleIndex = 1;
        private readonly string _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        private const long MaxIntegralValuePreservingPrecision = 999999999999999L;
        private long _sharedStringCount = 0;
        private readonly Dictionary<string, long> _sharedStringIndices = [];
        private SharedStringTablePart? _sharedStringTablePart;

        internal async Task Create(DbDataReader reader, string?[]? worksheetNames = null)
        {
            // https://learn.microsoft.com/en-us/office/open-xml/spreadsheet/working-with-sheets?tabs=cs
            using var document = SpreadsheetDocument.Create(_filePath, SpreadsheetDocumentType.Workbook);
            var workbookPart = InitializeWorkbook(document);

            var sheets = workbookPart.Workbook!.AppendChild(new Sheets());

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
                ConfigureCellValue(cell, typeof(string), row["ColumnName"]);
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
                ConfigureCellValue(cell, schemaTable.Rows[columnIndex - 1]["DataType"] as Type, reader[columnIndex - 1]);
            }
            return result;
        }

        // https://learn.microsoft.com/en-us/office/open-xml/spreadsheet/how-to-insert-text-into-a-cell-in-a-spreadsheet
        // altered to utilize a dictionary of indices for incrased performance
        private long InsertSharedStringItem(string text)
        {
            _sharedStringTablePart!.SharedStringTable ??= new SharedStringTable();

            if (_sharedStringIndices.TryGetValue(text, out var index))
            {
                return index;
            }

            index = _sharedStringCount;
            _sharedStringIndices[text] = index;
            _sharedStringTablePart.SharedStringTable.AppendChild(new SharedStringItem(new Text(text)));
            _sharedStringTablePart.SharedStringTable.Save();
            ++_sharedStringCount;

            return index;
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

        private WorkbookPart InitializeWorkbook(SpreadsheetDocument document)
        {
            // https://learn.microsoft.com/en-us/office/open-xml/spreadsheet/working-with-sheets?tabs=cs
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            _sharedStringTablePart = null;
            if (workbookPart.GetPartsOfType<SharedStringTablePart>().Any())
            {
                _sharedStringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
            }
            else
            {
                _sharedStringTablePart = workbookPart.AddNewPart<SharedStringTablePart>();
            }

            // for formatting dates => https://stackoverflow.com/a/31874753
            var workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
            workbookStylesPart.Stylesheet = new Stylesheet
            {
                CellStyleFormats = new CellStyleFormats(new CellFormat()),
                Borders = new Borders(new Border()),
                Fills = new Fills(new Fill()),
                Fonts = new Fonts(new Font()),
                CellFormats = new CellFormats
                (
                    new CellFormat(), // default: index 0
                    new CellFormat    // dates: index 1 (DateStyleIndex)
                    {
                        NumberFormatId = DateNumberFormatId,
                        ApplyNumberFormat = true
                    }
                )
            };

            return workbookPart;
        }

        private void ConfigureCellValue(Cell cell, Type? type, object? value)
        {
            type ??= typeof(string);
            if (value == null || DBNull.Value.Equals(value))
            {
                ConfigureCellValue(cell, null);
                return;
            }
            if (typeof(bool).Equals(type))
            {
                ConfigureCellValue(cell, (bool)value);
                return;
            }
            if (typeof(DateTime).Equals(type))
            {
                ConfigureCellValue(cell, (DateTime)value);
                return;
            }
            if (typeof(DateTimeOffset).Equals(type))
            {
                ConfigureCellValue(cell, (DateTimeOffset)value);
                return;
            }
            if (typeof(byte).Equals(type))
            {
                ConfigureCellValue(cell, (byte)value);
                return;
            }
            if (typeof(short).Equals(type))
            {
                ConfigureCellValue(cell, (short)value);
                return;
            }
            if (typeof(ushort).Equals(type))
            {
                ConfigureCellValue(cell, (ushort)value);
                return;
            }
            if (typeof(int).Equals(type))
            {
                ConfigureCellValue(cell, (int)value);
                return;
            }
            if (typeof(uint).Equals(type))
            {
                ConfigureCellValue(cell, (uint)value);
                return;
            }
            if (typeof(long).Equals(type))
            {
                ConfigureCellValue(cell, (long)value);
                return;
            }
            if (typeof(ulong).Equals(type))
            {
                ConfigureCellValue(cell, (ulong)value);
                return;
            }
            if (typeof(Int128).Equals(type))
            {
                ConfigureCellValue(cell, (Int128)value);
                return;
            }
            if (typeof(UInt128).Equals(type))
            {
                ConfigureCellValue(cell, (UInt128)value);
                return;
            }
            if (typeof(decimal).Equals(type))
            {
                ConfigureCellValue(cell, (decimal)value);
                return;
            }
            if (typeof(double).Equals(type))
            {
                ConfigureCellValue(cell, (double)value);
                return;
            }
            if (typeof(float).Equals(type))
            {
                ConfigureCellValue(cell, (float)value);
                return;
            }
            // handle everything else as string
            ConfigureCellValue(cell, value?.ToString());
        }

        private void ConfigureCellValue(Cell cell, string? value)
        {
            var sharedStringIndex = InsertSharedStringItem(value?.ToString() ?? string.Empty);
            cell.CellValue = new CellValue(sharedStringIndex.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        }

        private static void ConfigureCellValue(Cell cell, bool value)
        {
            cell.CellValue = new CellValue(value);
            cell.DataType = new EnumValue<CellValues>(CellValues.Boolean);
        }

        private static void ConfigureCellValue(Cell cell, DateTime value)
        {
            cell.CellValue = new CellValue(value);
            cell.DataType = new EnumValue<CellValues>(CellValues.Date);
            cell.StyleIndex = DateStyleIndex;
        }

        private static void ConfigureCellValue(Cell cell, DateTimeOffset value)
        {
            cell.CellValue = new CellValue(value);
            cell.DataType = new EnumValue<CellValues>(CellValues.Date);
            cell.StyleIndex = DateStyleIndex;
        }

        private static void ConfigureCellValue(Cell cell, byte value)
        {
            cell.CellValue = new CellValue(value);
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        private static void ConfigureCellValue(Cell cell, short value)
        {
            cell.CellValue = new CellValue(value);
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        private static void ConfigureCellValue(Cell cell, ushort value)
        {
            cell.CellValue = new CellValue(value);
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        private static void ConfigureCellValue(Cell cell, int value)
        {
            cell.CellValue = new CellValue(value);
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        private static void ConfigureCellValue(Cell cell, uint value)
        {
            cell.CellValue = new CellValue(value.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        private static void ConfigureCellValue(Cell cell, long value)
        {
            cell.CellValue = new CellValue(value.ToString());
            cell.DataType = new EnumValue<CellValues>(value > MaxIntegralValuePreservingPrecision || value < -MaxIntegralValuePreservingPrecision ? CellValues.String : CellValues.Number);
        }

        private static void ConfigureCellValue(Cell cell, ulong value)
        {
            cell.CellValue = new CellValue(value.ToString());
            cell.DataType = new EnumValue<CellValues>(value > MaxIntegralValuePreservingPrecision ? CellValues.String : CellValues.Number);
        }

        private static void ConfigureCellValue(Cell cell, Int128 value)
        {
            cell.CellValue = new CellValue(value.ToString());
            cell.DataType = new EnumValue<CellValues>(value > MaxIntegralValuePreservingPrecision || value < -MaxIntegralValuePreservingPrecision ? CellValues.String : CellValues.Number);
        }

        private static void ConfigureCellValue(Cell cell, UInt128 value)
        {
            cell.CellValue = new CellValue(value.ToString());
            cell.DataType = new EnumValue<CellValues>(value > MaxIntegralValuePreservingPrecision ? CellValues.String : CellValues.Number);
        }

        private static void ConfigureCellValue(Cell cell, decimal value)
        {
            cell.CellValue = new CellValue(value);
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        private static void ConfigureCellValue(Cell cell, double value)
        {
            cell.CellValue = new CellValue(value);
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        private static void ConfigureCellValue(Cell cell, float value)
        {
            cell.CellValue = new CellValue(value);
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }
    }
}
