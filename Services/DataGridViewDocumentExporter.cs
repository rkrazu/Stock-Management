using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Stock_Managemnet.Services
{
    public static class DataGridViewDocumentExporter
    {
        public static void Print(DataGridView grid, string title, IWin32Window owner)
        {
            TableDocument document;
            if (!TryCreateDocument(grid, title, owner, out document))
                return;

            var printDocument = new PrintDocument();
            printDocument.DocumentName = document.Title;
            printDocument.DefaultPageSettings.Landscape = document.Landscape;
            printDocument.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);
            ApplyA4(printDocument);

            var page = 0;
            printDocument.BeginPrint += (s, e) => page = 0;
            printDocument.PrintPage += (s, e) =>
            {
                var more = document.DrawPage(e.Graphics, e.MarginBounds, page);
                page++;
                e.HasMorePages = more;
            };

            try
            {
                using (var preview = new PrintPreviewDialog())
                {
                    preview.Document = printDocument;
                    preview.Width = 1040;
                    preview.Height = 820;
                    preview.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Print failed.\r\n\r\n" + ex.Message, "Print", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static bool ExportPdf(DataGridView grid, string title, IWin32Window owner)
        {
            TableDocument document;
            if (!TryCreateDocument(grid, title, owner, out document))
                return false;

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "PDF (*.pdf)|*.pdf";
                dialog.DefaultExt = "pdf";
                dialog.AddExtension = true;
                dialog.FileName = BuildPdfFileName(title);

                if (dialog.ShowDialog(owner) != DialogResult.OK)
                    return false;

                try
                {
                    var pageSize = document.PageSizeHundredths;
                    const int scale = 2;
                    var pixelWidth = Math.Max(1, (int)Math.Round(pageSize.Width * 96d / 100d * scale));
                    var pixelHeight = Math.Max(1, (int)Math.Round(pageSize.Height * 96d / 100d * scale));
                    var content = new Rectangle(50, 50, pageSize.Width - 100, pageSize.Height - 100);
                    var pages = new List<byte[]>();

                    for (var page = 0; page < document.PageCount; page++)
                    {
                        using (var bitmap = new Bitmap(pixelWidth, pixelHeight))
                        using (var graphics = Graphics.FromImage(bitmap))
                        {
                            graphics.Clear(Color.White);
                            graphics.ScaleTransform(scale, scale);
                            graphics.PageUnit = GraphicsUnit.Display;
                            graphics.PageScale = 96f / 100f;
                            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                            document.DrawPage(graphics, content, page);
                            pages.Add(EncodeJpeg(bitmap, 85L));
                        }
                    }

                    var pageWidthPt = document.Landscape ? 841.89 : 595.28;
                    var pageHeightPt = document.Landscape ? 595.28 : 841.89;
                    WriteJpegPdf(dialog.FileName, pages, pixelWidth, pixelHeight, pageWidthPt, pageHeightPt);
                    MessageBox.Show(owner, "PDF saved successfully.", "PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(owner, "Failed to save PDF.\r\n\r\n" + ex.Message, "PDF", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
        }

        private static bool TryCreateDocument(DataGridView grid, string title, IWin32Window owner, out TableDocument document)
        {
            document = null;
            if (grid == null)
                return false;

            var columns = grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).OrderBy(c => c.DisplayIndex).ToList();
            if (columns.Count == 0)
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

            document = TableDocument.Create(title, columns, dataRows);
            return true;
        }

        private static void ApplyA4(PrintDocument document)
        {
            foreach (PaperSize paperSize in document.PrinterSettings.PaperSizes)
            {
                if (paperSize.Kind == PaperKind.A4)
                {
                    document.DefaultPageSettings.PaperSize = paperSize;
                    break;
                }
            }
        }

        private static string BuildPdfFileName(string title)
        {
            var name = string.IsNullOrWhiteSpace(title) ? "Export" : title.Trim();
            foreach (var invalid in Path.GetInvalidFileNameChars())
                name = name.Replace(invalid, '_');

            if (name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                name = name.Substring(0, name.Length - 4);

            var now = DateTime.Now;
            var stamp = now.ToString("dd-MMM-yyyy_hh-mm-ss", CultureInfo.InvariantCulture)
                + "-"
                + now.ToString("tt", CultureInfo.InvariantCulture).ToLowerInvariant();
            return name + "_" + stamp + ".pdf";
        }

        private static byte[] EncodeJpeg(Image image, long quality)
        {
            var codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
            using (var stream = new MemoryStream())
            {
                if (codec == null)
                {
                    image.Save(stream, ImageFormat.Jpeg);
                    return stream.ToArray();
                }

                using (var encoderParams = new EncoderParameters(1))
                {
                    encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                    image.Save(stream, codec, encoderParams);
                    return stream.ToArray();
                }
            }
        }

        private static void WriteJpegPdf(
            string filePath,
            IList<byte[]> pages,
            int pixelWidth,
            int pixelHeight,
            double pageWidthPt,
            double pageHeightPt)
        {
            var pageCount = pages.Count;
            var objectCount = 2 + (pageCount * 3);
            var offsets = new long[objectCount + 1];

            using (var stream = File.Create(filePath))
            using (var writer = new BinaryWriter(stream, Encoding.ASCII))
            {
                Action<string> writeAscii = text => writer.Write(Encoding.ASCII.GetBytes(text));

                writeAscii("%PDF-1.4\n");

                offsets[1] = stream.Position;
                writeAscii("1 0 obj<< /Type /Catalog /Pages 2 0 R >>endobj\n");

                var kids = new StringBuilder();
                for (var page = 0; page < pageCount; page++)
                {
                    if (page > 0)
                        kids.Append(' ');
                    kids.Append(3 + (page * 3));
                    kids.Append(" 0 R");
                }

                offsets[2] = stream.Position;
                writeAscii("2 0 obj<< /Type /Pages /Kids [" + kids + "] /Count " + pageCount.ToString(CultureInfo.InvariantCulture) + " >>endobj\n");

                for (var page = 0; page < pageCount; page++)
                {
                    var pageObject = 3 + (page * 3);
                    var contentObject = pageObject + 1;
                    var imageObject = pageObject + 2;

                    offsets[pageObject] = stream.Position;
                    writeAscii(string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} 0 obj<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {1:0.###} {2:0.###}] /Resources << /XObject << /Im0 {3} 0 R >> >> /Contents {4} 0 R >>endobj\n",
                        pageObject,
                        pageWidthPt,
                        pageHeightPt,
                        imageObject,
                        contentObject));

                    var content = string.Format(
                        CultureInfo.InvariantCulture,
                        "q {0:0.###} 0 0 {1:0.###} 0 0 cm /Im0 Do Q\n",
                        pageWidthPt,
                        pageHeightPt);
                    var contentBytes = Encoding.ASCII.GetBytes(content);

                    offsets[contentObject] = stream.Position;
                    writeAscii(contentObject.ToString(CultureInfo.InvariantCulture) + " 0 obj<< /Length " + contentBytes.Length.ToString(CultureInfo.InvariantCulture) + " >>stream\n");
                    writer.Write(contentBytes);
                    writeAscii("endstream\nendobj\n");

                    offsets[imageObject] = stream.Position;
                    writeAscii(string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} 0 obj<< /Type /XObject /Subtype /Image /Width {1} /Height {2} /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length {3} >>stream\n",
                        imageObject,
                        pixelWidth,
                        pixelHeight,
                        pages[page].Length));
                    writer.Write(pages[page]);
                    writeAscii("\nendstream\nendobj\n");
                }

                var xrefPos = stream.Position;
                writeAscii("xref\n0 " + (objectCount + 1).ToString(CultureInfo.InvariantCulture) + "\n");
                writeAscii("0000000000 65535 f \n");
                for (var i = 1; i <= objectCount; i++)
                    writeAscii(offsets[i].ToString("D10", CultureInfo.InvariantCulture) + " 00000 n \n");

                writeAscii("trailer<< /Size " + (objectCount + 1).ToString(CultureInfo.InvariantCulture) + " /Root 1 0 R >>\nstartxref\n" + xrefPos.ToString(CultureInfo.InvariantCulture) + "\n%%EOF\n");
            }
        }

        private sealed class TableDocument
        {
            private const int TitleHeight = 36;
            private const int HeaderHeight = 28;
            private const int RowHeight = 26;
            private const int FooterHeight = 24;
            private const int CellPad = 6;

            private static readonly Size PortraitPage = new Size(827, 1169);
            private static readonly Size LandscapePage = new Size(1169, 827);

            private readonly string[] _headers;
            private readonly string[][] _rows;
            private readonly bool[] _rightAlign;
            private readonly int[] _columnWidths;

            private TableDocument(string title, string[] headers, string[][] rows, bool[] rightAlign, int[] columnWidths, bool landscape)
            {
                Title = title;
                _headers = headers;
                _rows = rows;
                _rightAlign = rightAlign;
                _columnWidths = columnWidths;
                Landscape = landscape;
            }

            public string Title { get; }
            public bool Landscape { get; }
            public Size PageSizeHundredths => Landscape ? LandscapePage : PortraitPage;

            public int PageCount
            {
                get
                {
                    var rowsPerPage = RowsPerPage(ContentSize(PageSizeHundredths));
                    return Math.Max(1, (int)Math.Ceiling(_rows.Length / (double)rowsPerPage));
                }
            }

            public static TableDocument Create(string title, IList<DataGridViewColumn> columns, IList<DataGridViewRow> dataRows)
            {
                var safeTitle = string.IsNullOrWhiteSpace(title) ? "Export" : title.Trim();
                var headers = columns.Select(c => c.HeaderText ?? string.Empty).ToArray();
                var rightAlign = columns.Select(IsRightAligned).ToArray();
                var rows = new string[dataRows.Count][];
                for (var r = 0; r < dataRows.Count; r++)
                {
                    rows[r] = new string[columns.Count];
                    for (var c = 0; c < columns.Count; c++)
                    {
                        var value = dataRows[r].Cells[columns[c].Index].FormattedValue;
                        rows[r][c] = value == null || value == DBNull.Value
                            ? string.Empty
                            : Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;
                    }
                }

                int[] widths;
                using (var headerFont = new Font("Segoe UI", 9F, FontStyle.Bold))
                using (var cellFont = new Font("Segoe UI", 8.5F))
                using (var bitmap = new Bitmap(1, 1))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    widths = new int[columns.Count];
                    for (var c = 0; c < columns.Count; c++)
                    {
                        var width = Measure(graphics, headers[c], headerFont);
                        for (var r = 0; r < rows.Length; r++)
                            width = Math.Max(width, Measure(graphics, rows[r][c], cellFont));
                        widths[c] = width + (CellPad * 2);
                    }
                }

                var portraitContent = PortraitPage.Width - 100;
                var natural = widths.Sum();
                var landscape = natural > portraitContent;
                var contentWidth = (landscape ? LandscapePage.Width : PortraitPage.Width) - 100;
                ScaleWidths(widths, contentWidth);
                return new TableDocument(safeTitle, headers, rows, rightAlign, widths, landscape);
            }

            public bool DrawPage(Graphics graphics, Rectangle content, int pageIndex)
            {
                var rowsPerPage = RowsPerPage(content.Size);
                var start = pageIndex * rowsPerPage;
                var count = Math.Max(0, Math.Min(rowsPerPage, _rows.Length - start));

                using (var titleFont = new Font("Segoe UI", 12F, FontStyle.Bold))
                using (var headerFont = new Font("Segoe UI", 9F, FontStyle.Bold))
                using (var cellFont = new Font("Segoe UI", 8.5F))
                using (var footerFont = new Font("Segoe UI", 8F))
                using (var titleBrush = new SolidBrush(Color.FromArgb(17, 24, 39)))
                using (var textBrush = new SolidBrush(Color.FromArgb(31, 41, 55)))
                using (var headerBack = new SolidBrush(Color.FromArgb(243, 244, 246)))
                using (var linePen = new Pen(Color.FromArgb(229, 231, 235)))
                using (var format = new StringFormat
                {
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap,
                    LineAlignment = StringAlignment.Center
                })
                {
                    graphics.DrawString(Title, titleFont, titleBrush, new Rectangle(content.Left, content.Top, content.Width, TitleHeight), format);

                    var y = content.Top + TitleHeight;
                    graphics.FillRectangle(headerBack, content.Left, y, content.Width, HeaderHeight);
                    DrawRow(graphics, _headers, headerFont, textBrush, linePen, format, content.Left, y, HeaderHeight, isHeader: true);
                    y += HeaderHeight;

                    for (var i = 0; i < count; i++)
                    {
                        DrawRow(graphics, _rows[start + i], cellFont, textBrush, linePen, format, content.Left, y, RowHeight, isHeader: false);
                        y += RowHeight;
                    }

                    format.Alignment = StringAlignment.Far;
                    graphics.DrawString(
                        "Page " + (pageIndex + 1).ToString(CultureInfo.CurrentCulture) + " of " + PageCount.ToString(CultureInfo.CurrentCulture),
                        footerFont,
                        textBrush,
                        new Rectangle(content.Left, content.Bottom - FooterHeight, content.Width, FooterHeight),
                        format);
                }

                return start + count < _rows.Length;
            }

            private void DrawRow(
                Graphics graphics,
                string[] values,
                Font font,
                Brush textBrush,
                Pen linePen,
                StringFormat format,
                int left,
                int top,
                int height,
                bool isHeader)
            {
                var x = left;
                for (var c = 0; c < values.Length; c++)
                {
                    var width = _columnWidths[c];
                    var rect = new Rectangle(x + CellPad, top, Math.Max(1, width - (CellPad * 2)), height);
                    format.Alignment = !isHeader && _rightAlign[c] ? StringAlignment.Far : StringAlignment.Near;
                    graphics.DrawString(values[c] ?? string.Empty, font, textBrush, rect, format);
                    x += width;
                }

                graphics.DrawLine(linePen, left, top + height, left + _columnWidths.Sum(), top + height);
            }

            private static int RowsPerPage(Size content)
            {
                var body = content.Height - TitleHeight - FooterHeight - HeaderHeight;
                return Math.Max(1, body / RowHeight);
            }

            private static Size ContentSize(Size page)
            {
                return new Size(page.Width - 100, page.Height - 100);
            }

            private static int Measure(Graphics graphics, string text, Font font)
            {
                var size = graphics.MeasureString(string.IsNullOrEmpty(text) ? " " : text, font);
                return (int)Math.Ceiling(size.Width * 100f / 96f);
            }

            private static void ScaleWidths(int[] widths, int contentWidth)
            {
                var total = widths.Sum();
                if (total <= contentWidth || total == 0)
                    return;

                var scale = contentWidth / (double)total;
                var used = 0;
                for (var i = 0; i < widths.Length; i++)
                {
                    widths[i] = Math.Max(36, (int)Math.Floor(widths[i] * scale));
                    used += widths[i];
                }

                var overflow = used - contentWidth;
                for (var i = widths.Length - 1; overflow > 0 && i >= 0; i--)
                {
                    var reduce = Math.Min(overflow, Math.Max(0, widths[i] - 36));
                    widths[i] -= reduce;
                    overflow -= reduce;
                }
            }

            private static bool IsRightAligned(DataGridViewColumn column)
            {
                var alignment = column.DefaultCellStyle.Alignment;
                return alignment == DataGridViewContentAlignment.MiddleRight
                    || alignment == DataGridViewContentAlignment.TopRight
                    || alignment == DataGridViewContentAlignment.BottomRight;
            }
        }
    }
}
