using ClosedXML.Excel;
using FileExporter.Interfaces;
using FileExporter.Model;
using System.Globalization;
using System.Text.Json;

namespace FileExporter.Services
{
    public class ExportService : IExportService
    {
        public byte[] GenerateExcel(ExportRequestModel request)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(request.SheetName);

            for (int i = 0; i < request.Columns.Count; i++)
            {
                var column = request.Columns[i];
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = column.Header;
                cell.Style.Font.Bold = true;
            }

            for (int rowIndex = 0; rowIndex < request.Rows.Count; rowIndex++)
            {
                var row = request.Rows[rowIndex];

                for (int colIndex = 0; colIndex < request.Columns.Count; colIndex++)
                {
                    var column = request.Columns[colIndex];
                    var rawValue = row.TryGetValue(column.Key, out var val) ? val : null;

                    var cell = worksheet.Cell(rowIndex + 2, colIndex + 1);

                    WriteCellValue(cell, rawValue, column.Format, column.DataType);
                }
            }

            for (int i = 0; i < request.Columns.Count; i++)
            {
                var width = request.Columns[i].Width;
                if (width.HasValue)
                    worksheet.Column(i + 1).Width = width.Value;
                else
                    worksheet.Column(i + 1).AdjustToContents();
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static void WriteCellValue(IXLCell cell, object? value, string? format, string? dataType)
        {
            if (value is null)
            {
                cell.Value = string.Empty;
                return;
            }

            if (string.Equals(dataType, "text", StringComparison.OrdinalIgnoreCase))
            {
                cell.Value = value.ToString() ?? string.Empty;
                cell.Style.NumberFormat.Format = "@";
                return;
            }

            if (value is JsonElement jsonElement)
            {
                WriteJsonElement(cell, jsonElement, format, dataType);
                return;
            }

            switch (value)
            {
                case int intValue:
                    cell.Value = intValue;
                    break;

                case long longValue:
                    cell.Value = longValue;
                    break;

                case decimal decimalValue:
                    cell.Value = decimalValue;
                    ApplyNumberFormat(cell, format);
                    break;

                case double doubleValue:
                    cell.Value = doubleValue;
                    ApplyNumberFormat(cell, format);
                    break;

                case float floatValue:
                    cell.Value = floatValue;
                    ApplyNumberFormat(cell, format);
                    break;

                case bool boolValue:
                    cell.Value = boolValue ? "Evet" : "Hayır";
                    break;

                case DateTime dateTimeValue:
                    cell.Value = dateTimeValue;
                    cell.Style.DateFormat.Format = string.IsNullOrWhiteSpace(format)
                        ? "yyyy-MM-dd HH:mm:ss"
                        : format;
                    break;

                case string stringValue:
                    WriteStringValue(cell, stringValue, format, dataType);
                    break;

                default:
                    cell.Value = value.ToString() ?? string.Empty;
                    break;
            }
        }
        private static void WriteJsonElement(IXLCell cell, JsonElement jsonElement, string? format, string? dataType)
        {
            if (string.Equals(dataType, "text", StringComparison.OrdinalIgnoreCase))
            {
                cell.Value = jsonElement.ToString();
                cell.Style.NumberFormat.Format = "@";
                return;
            }

            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.True:
                    cell.Value = "Evet";
                    break;

                case JsonValueKind.False:
                    cell.Value = "Hayır";
                    break;

                case JsonValueKind.Number:
                    if (jsonElement.TryGetInt32(out var intValue))
                    {
                        cell.Value = intValue;
                    }
                    else if (jsonElement.TryGetInt64(out var longValue))
                    {
                        cell.Value = longValue;
                    }
                    else if (jsonElement.TryGetDecimal(out var decimalValue))
                    {
                        cell.Value = decimalValue;
                        ApplyNumberFormat(cell, format);
                    }
                    else if (jsonElement.TryGetDouble(out var doubleValue))
                    {
                        cell.Value = doubleValue;
                        ApplyNumberFormat(cell, format);
                    }
                    else
                    {
                        cell.Value = jsonElement.ToString();
                    }
                    break;

                case JsonValueKind.String:
                    WriteStringValue(cell, jsonElement.GetString(), format, dataType);
                    break;

                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    cell.Value = string.Empty;
                    break;

                default:
                    cell.Value = jsonElement.ToString();
                    break;
            }
        }
        private static void WriteStringValue(IXLCell cell, string? value, string? format, string? dataType)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                cell.Value = string.Empty;
                return;
            }

            if (string.Equals(dataType, "text", StringComparison.OrdinalIgnoreCase))
            {
                cell.Value = value;
                cell.Style.NumberFormat.Format = "@";
                return;
            }

            if (string.Equals(dataType, "boolean", StringComparison.OrdinalIgnoreCase) &&
                bool.TryParse(value, out var boolValue))
            {
                cell.Value = boolValue ? "Evet" : "Hayır";
                return;
            }

            if (string.Equals(dataType, "number", StringComparison.OrdinalIgnoreCase))
            {
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
                {
                    cell.Value = decimalValue;
                    ApplyNumberFormat(cell, format);
                    return;
                }

                cell.Value = value;
                return;
            }

            if (string.Equals(dataType, "datetime", StringComparison.OrdinalIgnoreCase) &&
                DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue))
            {
                cell.Value = dateValue;
                cell.Style.DateFormat.Format = string.IsNullOrWhiteSpace(format)
                    ? "yyyy-MM-dd HH:mm:ss"
                    : format;
                return;
            }

            // DataType verilmediyse düz text bırakmak daha güvenli
            cell.Value = value;
        }

        private static void ApplyNumberFormat(IXLCell cell, string? format)
        {
            if (!string.IsNullOrWhiteSpace(format))
            {
                cell.Style.NumberFormat.Format = format;
            }
        }
    }
}