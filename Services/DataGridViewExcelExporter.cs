using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace Stock_Managemnet.Services
{
    public static class DataGridViewExcelExporter
    {
        public static bool Export(DataGridView grid, string defaultFileName, IWin32Window owner)
        {
            if (grid == null)
                return false;

            var visibleColumns = grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).ToList();
            if (visibleColumns.Count == 0)
            {
                MessageBox.Show(owner, "No columns to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            var dataRows = grid.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow).ToList();
            if (dataRows.Count == 0)
            {
                MessageBox.Show(owner, "No data to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                dialog.DefaultExt = "xlsx";
                dialog.AddExtension = true;
                dialog.FileName = BuildDefaultFileName(defaultFileName);

                if (dialog.ShowDialog(owner) != DialogResult.OK)
                    return false;

                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add(SanitizeSheetName(defaultFileName));

                        for (var col = 0; col < visibleColumns.Count; col++)
                        {
                            var headerCell = worksheet.Cell(1, col + 1);
                            headerCell.Value = visibleColumns[col].HeaderText;
                            headerCell.Style.Font.Bold = true;
                            headerCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F3F4F6");
                        }

                        for (var rowIndex = 0; rowIndex < dataRows.Count; rowIndex++)
                        {
                            var gridRow = dataRows[rowIndex];
                            for (var col = 0; col < visibleColumns.Count; col++)
                            {
                                var column = visibleColumns[col];
                                var cell = worksheet.Cell(rowIndex + 2, col + 1);
                                WriteCellValue(cell, gridRow.Cells[column.Index].FormattedValue);
                            }
                        }

                        worksheet.Columns().AdjustToContents();
                        worksheet.SheetView.FreezeRows(1);
                        workbook.SaveAs(dialog.FileName);
                    }

                    MessageBox.Show(owner, "Exported successfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(owner, "Export failed.\r\n\r\n" + ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
        }

        private static string BuildDefaultFileName(string defaultFileName)
        {
            var name = string.IsNullOrWhiteSpace(defaultFileName) ? "Export" : defaultFileName.Trim();
            foreach (var invalid in System.IO.Path.GetInvalidFileNameChars())
                name = name.Replace(invalid, '_');

            if (!name.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                name += ".xlsx";

            return name;
        }

        private static string SanitizeSheetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Export";

            var sheet = name.Trim();
            foreach (var invalid in new[] { '\\', '/', '?', '*', '[', ']', ':' })
                sheet = sheet.Replace(invalid, '_');

            if (sheet.Length > 31)
                sheet = sheet.Substring(0, 31);

            return sheet;
        }

        private static void WriteCellValue(IXLCell cell, object value)
        {
            if (value == null || value == DBNull.Value)
            {
                cell.Value = string.Empty;
                return;
            }

            if (value is bool boolean)
            {
                cell.Value = boolean ? "Yes" : "No";
                return;
            }

            if (value is DateTime dateTime)
            {
                cell.Value = dateTime;
                cell.Style.DateFormat.Format = "dd-MMM-yyyy HH:mm";
                return;
            }

            var text = Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out var number))
            {
                cell.Value = number;
                return;
            }

            if (DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out var parsedDate))
            {
                cell.Value = parsedDate;
                cell.Style.DateFormat.Format = "dd-MMM-yyyy HH:mm";
                return;
            }

            cell.Value = text;
        }
    }
}
