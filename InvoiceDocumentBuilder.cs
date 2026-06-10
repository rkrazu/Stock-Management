using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Stock_Managemnet.Models;

namespace Stock_Managemnet
{
    public static class InvoiceDocumentBuilder
    {
        public static string BuildText(Invoice invoice, bool isDraft)
        {
            const int lineWidth = 78;
            var lines = new System.Text.StringBuilder();
            lines.AppendLine("STOCK MANAGEMENT");
            lines.AppendLine(isDraft ? "INVOICE PREVIEW" : "INVOICE");
            lines.AppendLine(new string('-', lineWidth));
            lines.AppendLine(isDraft
                ? "Invoice #:   (assigned on submit)"
                : $"Invoice #:   {invoice.InvoiceNumber}");
            lines.AppendLine($"Date:        {invoice.CreatedAt:g}");
            lines.AppendLine(new string('-', lineWidth));
            lines.AppendLine("Bill To:");
            lines.AppendLine(string.IsNullOrWhiteSpace(invoice.CustomerName) ? "  (No customer selected)" : $"  {invoice.CustomerName}");

            if (!string.IsNullOrWhiteSpace(invoice.CustomerPhone))
                lines.AppendLine($"  Phone: {invoice.CustomerPhone}");
            if (!string.IsNullOrWhiteSpace(invoice.CustomerAddress))
                lines.AppendLine($"  Address: {invoice.CustomerAddress}");

            lines.AppendLine(new string('-', lineWidth));
            lines.AppendLine(
                $"{Pad("SKU", 16)}" +
                $"{Pad("Product", 28)}" +
                $"{Pad("Qty", 8, true)}" +
                $"{Pad("Price", 12, true)}" +
                $"{Pad("Total", 14, true)}");
            lines.AppendLine(new string('-', lineWidth));

            foreach (var item in invoice.Items)
            {
                lines.AppendLine(
                    $"{Pad(item.ProductSku, 16)}" +
                    $"{Pad(item.ProductName, 28)}" +
                    $"{Pad(item.Quantity.ToString(), 8, true)}" +
                    $"{Pad(item.UnitPrice.ToString("C2"), 12, true)}" +
                    $"{Pad(item.LineTotal.ToString("C2"), 14, true)}");
            }

            lines.AppendLine(new string('-', lineWidth));
            lines.AppendLine($"{Pad("TOTAL", 64)}{Pad(invoice.TotalAmount.ToString("C2"), 14, true)}");

            if (!string.IsNullOrWhiteSpace(invoice.Notes))
            {
                lines.AppendLine();
                lines.AppendLine("Notes:");
                lines.AppendLine(invoice.Notes);
            }

            return lines.ToString();
        }

        public static void Print(Invoice invoice, bool isDraft)
        {
            var text = BuildText(invoice, isDraft);
            var document = new PrintDocument();
            document.DocumentName = isDraft ? "Invoice Preview" : invoice.InvoiceNumber;
            document.PrintPage += (sender, e) =>
            {
                using (var font = new Font("Segoe UI", 11F))
                {
                    e.Graphics.DrawString(text, font, Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top);
                }
                e.HasMorePages = false;
            };

            using (var preview = new PrintPreviewDialog())
            {
                preview.Document = document;
                preview.Width = 900;
                preview.Height = 700;
                preview.ShowDialog();
            }
        }

        private static string Pad(string value, int width, bool rightAlign = false)
        {
            value = Truncate(value ?? string.Empty, width);
            return rightAlign ? value.PadLeft(width) : value.PadRight(width);
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength - 1) + "…";
        }
    }
}
