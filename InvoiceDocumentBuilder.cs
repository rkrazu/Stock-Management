using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public static class InvoiceDocumentBuilder
    {
        public enum InvoiceRenderProfile
        {
            Screen,
            Print
        }

        private static readonly Color BorderColor = Color.FromArgb(160, 160, 160);
        private static readonly Color TitleBarColor = Color.FromArgb(220, 220, 220);
        private static readonly Color SectionBarColor = Color.FromArgb(232, 196, 196);
        private static readonly Color TotalRowColor = Color.FromArgb(255, 242, 153);

        public static Size PageSize => InvoicePaperAssets.PageSize;

        public static int MeasurePageHeight(InvoiceRenderProfile profile = InvoiceRenderProfile.Screen)
        {
            return PageSize.Height;
        }

        public static int MeasureHeight(Invoice invoice, int width, InvoiceRenderProfile profile = InvoiceRenderProfile.Screen)
        {
            return MeasurePageHeight(profile);
        }

        public static void Paint(
            Graphics graphics,
            Rectangle bounds,
            Invoice invoice,
            bool isDraft,
            InvoiceRenderProfile profile = InvoiceRenderProfile.Screen)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            InvoicePaperAssets.DrawPageBackground(graphics, bounds);

            var contentBounds = InvoicePaperAssets.GetContentBounds(bounds.Width, bounds.Height);
            contentBounds.Offset(bounds.Left, bounds.Top);

            var layout = BuildLayout(invoice, contentBounds.Width, profile);
            var state = graphics.Save();
            graphics.SetClip(contentBounds);
            graphics.TranslateTransform(contentBounds.Left, contentBounds.Top);
            DrawDocument(graphics, layout, invoice, isDraft, profile);
            graphics.Restore(state);
        }

        public static void Print(Invoice invoice, bool isDraft)
        {
            var document = new PrintDocument();
            document.DocumentName = isDraft ? "Invoice Preview" : invoice.InvoiceNumber;
            document.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            document.DefaultPageSettings.Landscape = false;

            foreach (PaperSize paperSize in document.PrinterSettings.PaperSizes)
            {
                if (paperSize.Kind == PaperKind.A4)
                {
                    document.DefaultPageSettings.PaperSize = paperSize;
                    break;
                }
            }

            document.PrintPage += (sender, e) =>
            {
                Paint(
                    e.Graphics,
                    e.PageBounds,
                    invoice,
                    isDraft,
                    InvoiceRenderProfile.Print);
                e.HasMorePages = false;
            };

            using (var preview = new PrintPreviewDialog())
            {
                preview.Document = document;
                preview.Width = 1040;
                preview.Height = 820;
                preview.ShowDialog();
            }
        }

        public static bool ExportPdf(Invoice invoice, bool isDraft, IWin32Window owner)
        {
            if (invoice == null)
                return false;

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "PDF (*.pdf)|*.pdf";
                dialog.DefaultExt = "pdf";
                dialog.AddExtension = true;
                dialog.FileName = BuildPdfFileName(invoice, isDraft);

                if (dialog.ShowDialog(owner) != DialogResult.OK)
                    return false;

                try
                {
                    SavePdf(invoice, isDraft, dialog.FileName);
                    MessageBox.Show(
                        owner,
                        "PDF saved successfully.",
                        "PDF",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        owner,
                        "Failed to save PDF.\r\n\r\n" + ex.Message,
                        "PDF",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }
            }
        }

        public static void SavePdf(Invoice invoice, bool isDraft, string filePath)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path is required.", nameof(filePath));

            // Same logical page as the invoice popup (pad aspect + Screen profile),
            // then ScaleTransform for sharper output without changing relative layout/fonts.
            const int referenceWidth = 900;
            const int qualityScale = 2;
            var pageSize = InvoicePaperAssets.GetLogicalPageSize(referenceWidth);
            var width = Math.Max(1, pageSize.Width * qualityScale);
            var height = Math.Max(1, pageSize.Height * qualityScale);

            using (var bitmap = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.ScaleTransform(qualityScale, qualityScale);
                Paint(
                    graphics,
                    new Rectangle(0, 0, pageSize.Width, pageSize.Height),
                    invoice,
                    isDraft,
                    InvoiceRenderProfile.Screen);

                var jpegBytes = EncodeJpeg(bitmap, quality: 92L);
                WriteJpegPdf(filePath, jpegBytes, width, height);
            }
        }

        private static string BuildPdfFileName(Invoice invoice, bool isDraft)
        {
            var number = isDraft || string.IsNullOrWhiteSpace(invoice.InvoiceNumber)
                ? "Preview"
                : invoice.InvoiceNumber.Trim();

            foreach (var invalid in Path.GetInvalidFileNameChars())
                number = number.Replace(invalid, '_');

            return $"Invoice_{number}.pdf";
        }

        private static byte[] EncodeJpeg(Image image, long quality)
        {
            var codec = ImageCodecInfo.GetImageEncoders()
                .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
            if (codec == null)
            {
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Jpeg);
                    return stream.ToArray();
                }
            }

            using (var stream = new MemoryStream())
            using (var encoderParams = new EncoderParameters(1))
            {
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                image.Save(stream, codec, encoderParams);
                return stream.ToArray();
            }
        }

        private static void WriteJpegPdf(string filePath, byte[] jpegBytes, int pixelWidth, int pixelHeight)
        {
            const double pageWidthPt = 595.28;
            var pageHeightPt = pageWidthPt * pixelHeight / (double)pixelWidth;

            using (var stream = File.Create(filePath))
            using (var writer = new BinaryWriter(stream, Encoding.ASCII))
            {
                var offsets = new long[6];

                Action<string> writeAscii = text =>
                {
                    var bytes = Encoding.ASCII.GetBytes(text);
                    writer.Write(bytes);
                };

                writeAscii("%PDF-1.4\n");

                offsets[1] = stream.Position;
                writeAscii("1 0 obj<< /Type /Catalog /Pages 2 0 R >>endobj\n");

                offsets[2] = stream.Position;
                writeAscii("2 0 obj<< /Type /Pages /Kids [3 0 R] /Count 1 >>endobj\n");

                offsets[3] = stream.Position;
                writeAscii(string.Format(
                    CultureInfo.InvariantCulture,
                    "3 0 obj<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {0:0.###} {1:0.###}] /Resources << /XObject << /Im0 5 0 R >> >> /Contents 4 0 R >>endobj\n",
                    pageWidthPt,
                    pageHeightPt));

                var content = string.Format(
                    CultureInfo.InvariantCulture,
                    "q {0:0.###} 0 0 {1:0.###} 0 0 cm /Im0 Do Q\n",
                    pageWidthPt,
                    pageHeightPt);
                var contentBytes = Encoding.ASCII.GetBytes(content);

                offsets[4] = stream.Position;
                writeAscii($"4 0 obj<< /Length {contentBytes.Length} >>stream\n");
                writer.Write(contentBytes);
                writeAscii("endstream\nendobj\n");

                offsets[5] = stream.Position;
                writeAscii(string.Format(
                    CultureInfo.InvariantCulture,
                    "5 0 obj<< /Type /XObject /Subtype /Image /Width {0} /Height {1} /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length {2} >>stream\n",
                    pixelWidth,
                    pixelHeight,
                    jpegBytes.Length));
                writer.Write(jpegBytes);
                writeAscii("\nendstream\nendobj\n");

                var xrefPos = stream.Position;
                writeAscii("xref\n0 6\n");
                writeAscii("0000000000 65535 f \n");
                for (var i = 1; i <= 5; i++)
                    writeAscii($"{offsets[i]:D10} 00000 n \n");

                writeAscii($"trailer<< /Size 6 /Root 1 0 R >>\nstartxref\n{xrefPos}\n%%EOF\n");
            }
        }

        private static InvoiceFonts CreateFonts(InvoiceRenderProfile profile)
        {
            if (profile == InvoiceRenderProfile.Print)
            {
                return new InvoiceFonts
                {
                    Company = new Font("Segoe UI", 26F, FontStyle.Bold),
                    Title = new Font("Segoe UI", 18F, FontStyle.Bold),
                    Label = new Font("Segoe UI", 13F, FontStyle.Bold),
                    Text = new Font("Segoe UI", 13F),
                    Cell = new Font("Segoe UI", 11F),
                    Small = new Font("Segoe UI", 12F)
                };
            }

            return new InvoiceFonts
            {
                Company = new Font("Segoe UI", 22F, FontStyle.Bold),
                Title = new Font("Segoe UI", 15F, FontStyle.Bold),
                Label = new Font("Segoe UI", 11F, FontStyle.Bold),
                Text = new Font("Segoe UI", 11F),
                Cell = new Font("Segoe UI", 9.5F),
                Small = new Font("Segoe UI", 10F)
            };
        }

        private static void DrawDocument(
            Graphics graphics,
            DocumentLayout layout,
            Invoice invoice,
            bool isDraft,
            InvoiceRenderProfile profile)
        {
            using (var fonts = CreateFonts(profile))
            using (var borderPen = new Pen(BorderColor))
            {
                var y = 0;
                var width = layout.Width;
                var pad = layout.HorizontalPadding;
                var contentWidth = width - (pad * 2);

                if (!InvoicePaperAssets.HasPad)
                {
                    BrandAssets.DrawCompanyHeader(graphics, new Rectangle(0, y, width, layout.CompanyHeaderHeight), fonts.Company);
                    y += layout.CompanyHeaderHeight + 4;
                }

                FillBar(graphics, new Rectangle(0, y, width, layout.TitleBarHeight), TitleBarColor);
                DrawCenteredText(
                    graphics,
                    isDraft ? "INVOICE PREVIEW" : "INVOICE",
                    fonts.Title,
                    Brushes.Black,
                    new Rectangle(0, y, width, layout.TitleBarHeight));
                y += layout.TitleBarHeight;
                graphics.DrawLine(borderPen, 0, y, width, y);

                y += layout.SectionGap;
                var invoiceNo = isDraft ? "(assigned on submit)" : (invoice.InvoiceNumber ?? string.Empty);
                var halfWidth = contentWidth / 2;
                var rightHalfWidth = contentWidth - halfWidth;
                DrawLabelValue(graphics, fonts.Label, fonts.Text, "INVOICE NO:", invoiceNo, pad, y, halfWidth);
                DrawLabelValue(graphics, fonts.Label, fonts.Text, "Date:", invoice.CreatedAt.ToString("dd-MMM-yyyy"), pad + halfWidth, y, rightHalfWidth, rightAlign: true);
                y += layout.MetaRowHeight;
                graphics.DrawLine(borderPen, 0, y, width, y);

                y += layout.SectionGap;
                var customerName = string.IsNullOrWhiteSpace(invoice.CustomerName) ? "(No customer selected)" : invoice.CustomerName;
                var customerWidth = (contentWidth * 2) / 3;
                var mobileWidth = contentWidth - customerWidth;
                DrawLabelValue(graphics, fonts.Label, fonts.Text, "Customer:", customerName, pad, y, customerWidth, valueBold: true);
                DrawLabelValue(graphics, fonts.Label, fonts.Text, "Mobile:", invoice.CustomerPhone ?? string.Empty, pad + customerWidth, y, mobileWidth, rightAlign: true);
                y += layout.InfoRowHeight;

                DrawWrappedLabelValue(
                    graphics,
                    fonts.Label,
                    fonts.Text,
                    "Destination:",
                    FlattenSingleLine(invoice.CustomerAddress),
                    pad,
                    y,
                    contentWidth,
                    layout.MultilineFieldHeight);
                y += layout.MultilineFieldHeight;

                DrawWrappedLabelValue(
                    graphics,
                    fonts.Label,
                    fonts.Text,
                    "Note:",
                    FlattenSingleLine(invoice.Notes),
                    pad,
                    y,
                    contentWidth,
                    layout.MultilineFieldHeight);
                y += layout.MultilineFieldHeight;
                graphics.DrawLine(borderPen, 0, y, width, y);

                y += 2;
                FillBar(graphics, new Rectangle(0, y, width, layout.SectionBarHeight), SectionBarColor);
                DrawCenteredText(graphics, "GOODS DETAILS", fonts.Title, Brushes.Black, new Rectangle(0, y, width, layout.SectionBarHeight));
                y += layout.SectionBarHeight;

                layout.TableTop = y;
                DrawTableHeader(graphics, layout, y, fonts.Label, borderPen);
                y += layout.RowHeight;

                var rowIndex = 1;
                foreach (var item in invoice.Items ?? Enumerable.Empty<InvoiceLineItem>())
                {
                    var rowRect = new Rectangle(0, y, width, layout.RowHeight);
                    if (rowIndex % 2 == 0)
                        graphics.FillRectangle(Brushes.WhiteSmoke, rowRect);

                    DrawTableRow(graphics, layout, y, fonts.Cell, item, rowIndex);
                    graphics.DrawLine(borderPen, 0, y + layout.RowHeight, width, y + layout.RowHeight);
                    y += layout.RowHeight;
                    rowIndex++;
                }

                var totalQty = (invoice.Items ?? Enumerable.Empty<InvoiceLineItem>()).Sum(i => i.Quantity);
                FillBar(graphics, new Rectangle(0, y, width, layout.RowHeight), TotalRowColor);
                DrawTotalRow(graphics, layout, y, fonts.Label, fonts.Cell, totalQty, invoice.TotalAmount);
                graphics.DrawRectangle(borderPen, 0, layout.TableTop, width, y + layout.RowHeight - layout.TableTop);
                y += layout.RowHeight + layout.SectionGap;

                var gap = layout.SignatureGap;
                var signWidth = (width - gap) / 2;
                DrawSignatureBox(graphics, borderPen, fonts.Label, fonts.Small, "Authorized Person", new Rectangle(0, y, signWidth, layout.SignatureHeight));
                DrawSignatureBox(graphics, borderPen, fonts.Label, fonts.Small, "Received by (Customer)", new Rectangle(signWidth + gap, y, signWidth, layout.SignatureHeight));
                y += layout.SignatureHeight + layout.SectionGap;

                graphics.DrawLine(borderPen, 0, y, width, y);
                y += layout.SectionGap;
                var footerHalfWidth = contentWidth / 2;
                var footerRightWidth = contentWidth - footerHalfWidth;
                DrawLabelValue(
                    graphics,
                    fonts.Label,
                    fonts.Small,
                    "ENTRY TIME:",
                    FormatDateTime12Hour(invoice.CreatedAt),
                    pad,
                    y,
                    footerHalfWidth);
                DrawLabelValue(
                    graphics,
                    fonts.Label,
                    fonts.Small,
                    "PRINT TIME:",
                    FormatDateTime12Hour(DateTime.Now),
                    pad + footerHalfWidth,
                    y,
                    footerRightWidth,
                    rightAlign: true);
            }
        }

        private static DocumentLayout BuildLayout(Invoice invoice, int width, InvoiceRenderProfile profile)
        {
            width = Math.Max(480, width);
            var itemCount = Math.Max(1, invoice?.Items?.Count ?? 0);
            var isPrint = profile == InvoiceRenderProfile.Print;
            var useLetterhead = InvoicePaperAssets.HasPad;

            var layout = new DocumentLayout
            {
                Width = width,
                HorizontalPadding = isPrint ? 16 : 12,
                CompanyHeaderHeight = isPrint ? 52 : 44,
                TitleBarHeight = isPrint ? 44 : 38,
                SectionGap = isPrint ? 12 : 10,
                MetaRowHeight = isPrint ? 42 : 36,
                InfoRowHeight = isPrint ? 36 : 30,
                MultilineFieldHeight = isPrint ? 56 : 48,
                NoteRowHeight = isPrint ? 56 : 48,
                SectionBarHeight = isPrint ? 42 : 36,
                RowHeight = isPrint ? 40 : 34,
                SignatureHeight = isPrint ? 84 : 72,
                SignatureGap = isPrint ? 16 : 12,
                PriceWidth = isPrint ? 130 : 118,
                QtyWidth = isPrint ? 64 : 58,
                AmountWidth = isPrint ? 140 : 128
            };

            var headerOffset = useLetterhead ? 0 : layout.CompanyHeaderHeight + 4;

            layout.TableTop =
                headerOffset +
                layout.TitleBarHeight + layout.SectionGap +
                layout.MetaRowHeight + layout.SectionGap +
                layout.InfoRowHeight +
                layout.MultilineFieldHeight +
                layout.MultilineFieldHeight + 2 +
                layout.SectionBarHeight;

            layout.TotalHeight =
                layout.TableTop +
                layout.RowHeight +
                (itemCount * layout.RowHeight) +
                layout.RowHeight + layout.SectionGap +
                layout.SignatureHeight + layout.SectionGap +
                layout.SectionGap + (isPrint ? 30 : 24) + 12;

            return layout;
        }

        private static void DrawTableHeader(Graphics graphics, DocumentLayout layout, int y, Font font, Pen borderPen)
        {
            var x = 0;
            var rect = new Rectangle(0, y, layout.Width, layout.RowHeight);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(245, 245, 245)), rect);
            graphics.DrawLine(borderPen, 0, y, layout.Width, y);

            DrawCellText(graphics, "Product Info", font, new Rectangle(x, y, layout.ProductInfoWidth, layout.RowHeight), true);
            x += layout.ProductInfoWidth;
            DrawCellText(graphics, "Unit Price", font, new Rectangle(x, y, layout.PriceWidth, layout.RowHeight), true, rightAlign: true);
            x += layout.PriceWidth;
            DrawCellText(graphics, "Qty", font, new Rectangle(x, y, layout.QtyWidth, layout.RowHeight), true, rightAlign: true);
            x += layout.QtyWidth;
            DrawCellText(graphics, "Amount", font, new Rectangle(x, y, layout.AmountWidth, layout.RowHeight), true, rightAlign: true);
            graphics.DrawLine(borderPen, 0, y + layout.RowHeight, layout.Width, y + layout.RowHeight);
        }

        private static void DrawTableRow(Graphics graphics, DocumentLayout layout, int y, Font font, InvoiceLineItem item, int rowIndex)
        {
            var x = 0;
            DrawCellText(graphics, FormatProductInfo(rowIndex, item), font, new Rectangle(x, y, layout.ProductInfoWidth, layout.RowHeight));
            x += layout.ProductInfoWidth;
            DrawCellText(graphics, item.UnitPrice.ToString("C2"), font, new Rectangle(x, y, layout.PriceWidth, layout.RowHeight), false, rightAlign: true);
            x += layout.PriceWidth;
            DrawCellText(graphics, item.Quantity.ToString(), font, new Rectangle(x, y, layout.QtyWidth, layout.RowHeight), false, rightAlign: true);
            x += layout.QtyWidth;
            DrawCellText(graphics, item.LineTotal.ToString("C2"), font, new Rectangle(x, y, layout.AmountWidth, layout.RowHeight), false, rightAlign: true);
        }

        private static void DrawTotalRow(Graphics graphics, DocumentLayout layout, int y, Font fontBold, Font font, int totalQty, decimal totalAmount)
        {
            DrawCellText(graphics, "Total", fontBold, new Rectangle(0, y, layout.ProductInfoWidth, layout.RowHeight), true);
            var priceX = layout.ProductInfoWidth;
            DrawCellText(graphics, string.Empty, font, new Rectangle(priceX, y, layout.PriceWidth, layout.RowHeight));
            DrawCellText(graphics, totalQty.ToString(), fontBold, new Rectangle(priceX + layout.PriceWidth, y, layout.QtyWidth, layout.RowHeight), true, rightAlign: true);
            DrawCellText(graphics, totalAmount.ToString("C2"), fontBold, new Rectangle(priceX + layout.PriceWidth + layout.QtyWidth, y, layout.AmountWidth, layout.RowHeight), true, rightAlign: true);
        }

        private static string FormatProductInfo(int rowIndex, InvoiceLineItem item)
        {
            var sku = string.IsNullOrWhiteSpace(item?.ProductSku) ? "-" : item.ProductSku.Trim();
            var category = string.IsNullOrWhiteSpace(item?.ProductCategory) ? "-" : item.ProductCategory.Trim();
            var name = string.IsNullOrWhiteSpace(item?.ProductName) ? "-" : item.ProductName.Trim();
            return $"{rowIndex}. {sku} - {category} - {name}";
        }

        private static void DrawSignatureBox(Graphics graphics, Pen borderPen, Font fontBold, Font fontSmall, string title, Rectangle bounds)
        {
            graphics.DrawRectangle(borderPen, bounds);
            var titleRect = new Rectangle(bounds.Left + 10, bounds.Top + 10, bounds.Width - 20, 28);
            graphics.DrawString(title, fontBold, Brushes.Black, titleRect);
            var lineY = bounds.Bottom - 28;
            graphics.DrawLine(borderPen, bounds.Left + 10, lineY, bounds.Right - 10, lineY);
            graphics.DrawString("Signature", fontSmall, Brushes.Gray, bounds.Left + 10, lineY + 4);
        }

        private static void FillBar(Graphics graphics, Rectangle bounds, Color color)
        {
            using (var brush = new SolidBrush(color))
                graphics.FillRectangle(brush, bounds);
        }

        private static void DrawCenteredText(Graphics graphics, string text, Font font, Brush brush, Rectangle bounds)
        {
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            graphics.DrawString(text, font, brush, bounds, format);
        }

        private static string FlattenSingleLine(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var parts = value
                .Replace("\r\n", "\n")
                .Replace('\r', '\n')
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Trim())
                .Where(part => part.Length > 0);

            return string.Join(" ", parts);
        }

        private static string FormatDateTime12Hour(DateTime value) =>
            value.ToString("dd-MMM-yyyy hh:mm:ss tt");

        private static void DrawLabelValue(
            Graphics graphics,
            Font fontLabel,
            Font fontValue,
            string label,
            string value,
            int x,
            int y,
            int width,
            bool valueBold = false,
            bool rightAlign = false)
        {
            DrawWrappedLabelValue(
                graphics,
                fontLabel,
                fontValue,
                label,
                value,
                x,
                y,
                width,
                height: 28,
                valueBold: valueBold,
                singleLine: true,
                rightAlign: rightAlign);
        }

        private static void DrawWrappedLabelValue(
            Graphics graphics,
            Font fontLabel,
            Font fontValue,
            string label,
            string value,
            int x,
            int y,
            int width,
            int height,
            bool valueBold = false,
            bool singleLine = false,
            bool rightAlign = false)
        {
            var valueFont = valueBold ? new Font(fontValue, FontStyle.Bold) : fontValue;
            var safeValue = value ?? string.Empty;
            var labelText = label ?? string.Empty;
            var labelSize = graphics.MeasureString(labelText + " ", fontLabel);
            var valueSize = graphics.MeasureString(safeValue, valueFont);

            float labelX;
            float valueX;
            float valueWidth;

            if (rightAlign && singleLine)
            {
                var totalWidth = Math.Min(width, labelSize.Width + valueSize.Width);
                labelX = x + width - totalWidth;
                valueX = labelX + labelSize.Width;
                valueWidth = Math.Max(0, x + width - valueX);
            }
            else
            {
                labelX = x;
                valueX = x + labelSize.Width;
                valueWidth = Math.Max(0, width - labelSize.Width);
            }

            graphics.DrawString(labelText, fontLabel, Brushes.Black, new PointF(labelX, y));
            var valueRect = new RectangleF(valueX, y, valueWidth, Math.Max(1, height));

            var format = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = singleLine ? StringTrimming.EllipsisCharacter : StringTrimming.EllipsisWord,
                FormatFlags = singleLine
                    ? StringFormatFlags.NoWrap | StringFormatFlags.LineLimit
                    : StringFormatFlags.LineLimit
            };

            graphics.DrawString(safeValue, valueFont, Brushes.Black, valueRect, format);
            if (valueBold)
                valueFont.Dispose();
        }

        private static void DrawCellText(
            Graphics graphics,
            string text,
            Font font,
            Rectangle bounds,
            bool bold = false,
            bool rightAlign = false)
        {
            var cellFont = bold ? new Font(font, FontStyle.Bold) : font;
            var format = new StringFormat
            {
                Alignment = rightAlign ? StringAlignment.Far : StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };
            var rect = new Rectangle(bounds.Left + 4, bounds.Top, bounds.Width - 8, bounds.Height);
            graphics.DrawString(text ?? string.Empty, cellFont, Brushes.Black, rect, format);
            if (bold)
                cellFont.Dispose();
        }

        private sealed class InvoiceFonts : IDisposable
        {
            public Font Company { get; set; }
            public Font Title { get; set; }
            public Font Label { get; set; }
            public Font Text { get; set; }
            public Font Cell { get; set; }
            public Font Small { get; set; }

            public void Dispose()
            {
                Company?.Dispose();
                Title?.Dispose();
                Label?.Dispose();
                Text?.Dispose();
                Cell?.Dispose();
                Small?.Dispose();
            }
        }

        private sealed class DocumentLayout
        {
            public int Width { get; set; }
            public int HorizontalPadding { get; set; }
            public int CompanyHeaderHeight { get; set; }
            public int TitleBarHeight { get; set; }
            public int SectionGap { get; set; }
            public int MetaRowHeight { get; set; }
            public int InfoRowHeight { get; set; }
            public int MultilineFieldHeight { get; set; }
            public int NoteRowHeight { get; set; }
            public int SectionBarHeight { get; set; }
            public int RowHeight { get; set; }
            public int SignatureHeight { get; set; }
            public int SignatureGap { get; set; }
            public int TableTop { get; set; }
            public int TotalHeight { get; set; }
            public int PriceWidth { get; set; }
            public int QtyWidth { get; set; }
            public int AmountWidth { get; set; }
            public int ProductInfoWidth =>
                Math.Max(180, Width - PriceWidth - QtyWidth - AmountWidth);
        }
    }
}
