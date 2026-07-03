using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
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
                    Small = new Font("Segoe UI", 12F)
                };
            }

            return new InvoiceFonts
            {
                Company = new Font("Segoe UI", 22F, FontStyle.Bold),
                Title = new Font("Segoe UI", 15F, FontStyle.Bold),
                Label = new Font("Segoe UI", 11F, FontStyle.Bold),
                Text = new Font("Segoe UI", 11F),
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
                var thirdWidth = contentWidth / 3;
                var lastThirdWidth = contentWidth - (thirdWidth * 2);

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
                DrawLabelValue(graphics, fonts.Label, fonts.Text, "INVOICE NO:", invoiceNo, pad, y, thirdWidth);
                DrawLabelValue(graphics, fonts.Label, fonts.Text, "Date:", invoice.CreatedAt.ToString("dd-MMM-yyyy"), pad + thirdWidth, y, thirdWidth);
                DrawLabelValue(graphics, fonts.Label, fonts.Text, "Entry Time:", invoice.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), pad + (thirdWidth * 2), y, lastThirdWidth);
                y += layout.MetaRowHeight;
                graphics.DrawLine(borderPen, 0, y, width, y);

                y += layout.SectionGap;
                var customerName = string.IsNullOrWhiteSpace(invoice.CustomerName) ? "(No customer selected)" : invoice.CustomerName;
                DrawLabelValue(graphics, fonts.Label, fonts.Text, "Customer:", customerName, pad, y, contentWidth, valueBold: true);
                y += layout.InfoRowHeight;

                var halfWidth = contentWidth / 2;
                var secondHalfWidth = contentWidth - halfWidth;
                DrawLabelValue(graphics, fonts.Label, fonts.Text, "Mobile:", invoice.CustomerPhone ?? string.Empty, pad, y, halfWidth);
                DrawLabelValue(graphics, fonts.Label, fonts.Text, "Destination:", invoice.CustomerAddress ?? string.Empty, pad + halfWidth, y, secondHalfWidth);
                y += layout.InfoRowHeight;

                DrawLabelValue(graphics, fonts.Label, fonts.Text, "Note:", invoice.Notes ?? string.Empty, pad, y, contentWidth);
                y += layout.NoteRowHeight;
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

                    DrawTableRow(graphics, layout, y, fonts.Text, item, rowIndex);
                    graphics.DrawLine(borderPen, 0, y + layout.RowHeight, width, y + layout.RowHeight);
                    y += layout.RowHeight;
                    rowIndex++;
                }

                var totalQty = (invoice.Items ?? Enumerable.Empty<InvoiceLineItem>()).Sum(i => i.Quantity);
                FillBar(graphics, new Rectangle(0, y, width, layout.RowHeight), TotalRowColor);
                DrawTotalRow(graphics, layout, y, fonts.Label, fonts.Text, totalQty, invoice.TotalAmount);
                graphics.DrawRectangle(borderPen, 0, layout.TableTop, width, y + layout.RowHeight - layout.TableTop);
                y += layout.RowHeight + layout.SectionGap;

                var gap = layout.SignatureGap;
                var signWidth = (width - gap) / 2;
                DrawSignatureBox(graphics, borderPen, fonts.Label, fonts.Small, "Authorized Person", new Rectangle(0, y, signWidth, layout.SignatureHeight));
                DrawSignatureBox(graphics, borderPen, fonts.Label, fonts.Small, "Received by (Customer)", new Rectangle(signWidth + gap, y, signWidth, layout.SignatureHeight));
                y += layout.SignatureHeight + layout.SectionGap;

                graphics.DrawLine(borderPen, 0, y, width, y);
                y += layout.SectionGap;
                DrawLabelValue(graphics, fonts.Label, fonts.Small, "PRINT TIME:", DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss").ToUpperInvariant(), pad, y, contentWidth);
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
                NoteRowHeight = isPrint ? 40 : 34,
                SectionBarHeight = isPrint ? 42 : 36,
                RowHeight = isPrint ? 40 : 34,
                SignatureHeight = isPrint ? 84 : 72,
                SignatureGap = isPrint ? 16 : 12,
                SlWidth = isPrint ? 56 : 50,
                SkuWidth = isPrint ? 108 : 100,
                CategoryWidth = isPrint ? 128 : 120,
                PriceWidth = isPrint ? 104 : 96,
                QtyWidth = isPrint ? 76 : 68,
                AmountWidth = isPrint ? 112 : 104
            };

            var headerOffset = useLetterhead ? 0 : layout.CompanyHeaderHeight + 4;

            layout.TableTop =
                headerOffset +
                layout.TitleBarHeight + layout.SectionGap +
                layout.MetaRowHeight + layout.SectionGap +
                layout.InfoRowHeight +
                layout.InfoRowHeight +
                layout.NoteRowHeight + 2 +
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

            DrawCellText(graphics, "SL", font, new Rectangle(x, y, layout.SlWidth, layout.RowHeight), true);
            x += layout.SlWidth;
            DrawCellText(graphics, "SKU", font, new Rectangle(x, y, layout.SkuWidth, layout.RowHeight), true);
            x += layout.SkuWidth;
            DrawCellText(graphics, "Category", font, new Rectangle(x, y, layout.CategoryWidth, layout.RowHeight), true);
            x += layout.CategoryWidth;
            var nameWidth = layout.ProductNameWidth;
            DrawCellText(graphics, "Product Name", font, new Rectangle(x, y, nameWidth, layout.RowHeight), true);
            x += nameWidth;
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
            DrawCellText(graphics, rowIndex.ToString(), font, new Rectangle(x, y, layout.SlWidth, layout.RowHeight), false, rightAlign: true);
            x += layout.SlWidth;
            DrawCellText(graphics, item.ProductSku, font, new Rectangle(x, y, layout.SkuWidth, layout.RowHeight));
            x += layout.SkuWidth;
            DrawCellText(graphics, string.IsNullOrWhiteSpace(item.ProductCategory) ? "-" : item.ProductCategory, font, new Rectangle(x, y, layout.CategoryWidth, layout.RowHeight));
            x += layout.CategoryWidth;
            DrawCellText(graphics, item.ProductName, font, new Rectangle(x, y, layout.ProductNameWidth, layout.RowHeight));
            x += layout.ProductNameWidth;
            DrawCellText(graphics, item.UnitPrice.ToString("C2"), font, new Rectangle(x, y, layout.PriceWidth, layout.RowHeight), false, rightAlign: true);
            x += layout.PriceWidth;
            DrawCellText(graphics, item.Quantity.ToString(), font, new Rectangle(x, y, layout.QtyWidth, layout.RowHeight), false, rightAlign: true);
            x += layout.QtyWidth;
            DrawCellText(graphics, item.LineTotal.ToString("C2"), font, new Rectangle(x, y, layout.AmountWidth, layout.RowHeight), false, rightAlign: true);
        }

        private static void DrawTotalRow(Graphics graphics, DocumentLayout layout, int y, Font fontBold, Font font, int totalQty, decimal totalAmount)
        {
            var labelRect = new Rectangle(layout.SlWidth + layout.SkuWidth, y, layout.CategoryWidth + layout.ProductNameWidth, layout.RowHeight);
            DrawCellText(graphics, "Total", fontBold, labelRect, true);
            var priceX = layout.SlWidth + layout.SkuWidth + layout.CategoryWidth + layout.ProductNameWidth;
            DrawCellText(graphics, string.Empty, font, new Rectangle(priceX, y, layout.PriceWidth, layout.RowHeight));
            DrawCellText(graphics, totalQty.ToString(), fontBold, new Rectangle(priceX + layout.PriceWidth, y, layout.QtyWidth, layout.RowHeight), true, rightAlign: true);
            DrawCellText(graphics, totalAmount.ToString("C2"), fontBold, new Rectangle(priceX + layout.PriceWidth + layout.QtyWidth, y, layout.AmountWidth, layout.RowHeight), true, rightAlign: true);
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

        private static void DrawLabelValue(
            Graphics graphics,
            Font fontLabel,
            Font fontValue,
            string label,
            string value,
            int x,
            int y,
            int width,
            bool valueBold = false)
        {
            var valueFont = valueBold ? new Font(fontValue, FontStyle.Bold) : fontValue;
            graphics.DrawString(label, fontLabel, Brushes.Black, new PointF(x, y));
            var labelSize = graphics.MeasureString(label + " ", fontLabel);
            var valueRect = new RectangleF(x + labelSize.Width, y, Math.Max(0, width - labelSize.Width), 60);
            graphics.DrawString(value ?? string.Empty, valueFont, Brushes.Black, valueRect);
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
            var rect = new Rectangle(bounds.Left + 6, bounds.Top, bounds.Width - 12, bounds.Height);
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
            public Font Small { get; set; }

            public void Dispose()
            {
                Company?.Dispose();
                Title?.Dispose();
                Label?.Dispose();
                Text?.Dispose();
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
            public int NoteRowHeight { get; set; }
            public int SectionBarHeight { get; set; }
            public int RowHeight { get; set; }
            public int SignatureHeight { get; set; }
            public int SignatureGap { get; set; }
            public int TableTop { get; set; }
            public int TotalHeight { get; set; }
            public int SlWidth { get; set; }
            public int SkuWidth { get; set; }
            public int CategoryWidth { get; set; }
            public int PriceWidth { get; set; }
            public int QtyWidth { get; set; }
            public int AmountWidth { get; set; }
            public int ProductNameWidth =>
                Math.Max(140, Width - SlWidth - SkuWidth - CategoryWidth - PriceWidth - QtyWidth - AmountWidth);
        }
    }
}
