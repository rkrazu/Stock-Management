using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Stock_Managemnet.Controls
{
    public sealed class CogsReportControl : Control
    {
        private static readonly Color SalesCostColor = Color.FromArgb(59, 130, 246);
        private static readonly Color ExpenseColor = Color.FromArgb(239, 68, 68);
        private static readonly Color NetProfitColor = Color.FromArgb(34, 197, 94);
        private static readonly Color EmptyPieColor = Color.FromArgb(229, 231, 235);

        private decimal _totalSales;
        private decimal _totalSalesCost;
        private decimal _totalProfit;
        private decimal _totalExpense;
        private decimal _netProfit;

        public CogsReportControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 14F);
        }

        public void SetAmounts(decimal totalSales, decimal totalSalesCost, decimal totalProfit, decimal totalExpense)
        {
            _totalSales = totalSales;
            _totalSalesCost = totalSalesCost;
            _totalProfit = totalProfit;
            _totalExpense = totalExpense;
            _netProfit = totalProfit - totalExpense;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var titleFont = new Font(Font.FontFamily, 36F, FontStyle.Bold))
            using (var bodyFont = new Font(Font.FontFamily, 15F))
            using (var legendFont = new Font(Font.FontFamily, 12.5F))
            using (var ink = new SolidBrush(Color.Black))
            using (var borderPen = new Pen(Color.Black, 2F))
            using (var rulePen = new Pen(Color.Black, 1.5F))
            using (var measureFormat = CreateMeasureFormat())
            using (var nearFormat = CreateNearFormat())
            using (var farFormat = CreateFarFormat())
            {
                const float boxWidth = 620f;
                const float boxHeight = 310f;
                const float pieSize = 250f;
                const float gap = 64f;
                const float pieColumnWidth = 360f;
                const float padLeft = 40f;
                const float padRight = 28f;
                const float netProfitLabelGap = 10f;

                var titleSize = g.MeasureString("COGS", titleFont, int.MaxValue, measureFormat);
                var resultLabel = _netProfit < 0 ? "Net Loss" : "Net Profit";
                var netProfitLabelWidth = g.MeasureString(resultLabel, bodyFont, int.MaxValue, measureFormat).Width;
                var compositionWidth = boxWidth + gap + pieColumnWidth;
                var compositionHeight = titleSize.Height + 28f + Math.Max(boxHeight, pieSize + 120f);

                var originX = Math.Max(16f, (ClientSize.Width - compositionWidth) / 2f);
                var originY = Math.Max(16f, (ClientSize.Height - compositionHeight) / 2f);

                var boxX = originX;
                var boxY = originY + titleSize.Height + 28f;
                var pieX = boxX + boxWidth + gap;
                var pieY = boxY + 10f;

                // Title centered across the full composition (card + pie).
                g.DrawString(
                    "COGS",
                    titleFont,
                    ink,
                    originX + (compositionWidth - titleSize.Width) / 2f,
                    originY,
                    nearFormat);

                var box = new RectangleF(boxX, boxY, boxWidth, boxHeight);
                using (var path = CreateRoundedRectangle(box, 30f))
                    g.DrawPath(borderPen, path);

                // Amounts share one right edge; result label sits further right after that edge.
                var labelLeft = boxX + padLeft;
                var amountRight = boxX + boxWidth - padRight - netProfitLabelWidth - netProfitLabelGap;
                var equalsX = labelLeft + 250f;
                var amountLeft = equalsX + 28f;
                var lineHeight = 38f;
                var y = boxY + 34f;

                DrawLabeledAmount(g, bodyFont, ink, nearFormat, farFormat, "Total Sales", _totalSales, labelLeft, equalsX, amountLeft, amountRight, y);
                y += lineHeight;
                DrawLabeledAmount(g, bodyFont, ink, nearFormat, farFormat, "Total Sales Cost (-)", _totalSalesCost, labelLeft, equalsX, amountLeft, amountRight, y);
                y += lineHeight + 4f;
                DrawAmountRule(g, rulePen, amountLeft, amountRight, y);
                y += 14f;
                DrawLabeledAmount(g, bodyFont, ink, nearFormat, farFormat, "Total Profit", _totalProfit, labelLeft, equalsX, amountLeft, amountRight, y);
                y += lineHeight;
                DrawLabeledAmount(g, bodyFont, ink, nearFormat, farFormat, "Total Expense (-)", _totalExpense, labelLeft, equalsX, amountLeft, amountRight, y);
                y += lineHeight + 4f;
                DrawAmountRule(g, rulePen, amountLeft, amountRight, y);
                y += 16f;
                DrawNetResult(g, bodyFont, ink, nearFormat, farFormat, _netProfit, resultLabel, amountLeft, amountRight, netProfitLabelGap, y);

                DrawPieChart(g, legendFont, ink, nearFormat, pieX, pieY, pieSize);
            }
        }

        private void DrawPieChart(Graphics g, Font legendFont, Brush ink, StringFormat nearFormat, float x, float y, float size)
        {
            var slices = BuildSlices().ToList();
            var pieRectF = new RectangleF(x, y, size, size);
            var pieRect = Rectangle.Round(pieRectF);
            var sliceTotal = slices.Sum(s => s.Value);

            if (slices.Count == 0 || sliceTotal <= 0)
            {
                using (var empty = new SolidBrush(EmptyPieColor))
                using (var outline = new Pen(Color.FromArgb(156, 163, 175), 1.5f))
                {
                    g.FillEllipse(empty, pieRectF);
                    g.DrawEllipse(outline, pieRectF);
                }
            }
            else
            {
                var startAngle = -90f;
                foreach (var slice in slices)
                {
                    var sweep = (float)(slice.Value / sliceTotal * 360m);
                    if (sweep <= 0f)
                        continue;

                    using (var brush = new SolidBrush(slice.Color))
                    {
                        if (slices.Count == 1 || sweep >= 359.9f)
                            g.FillEllipse(brush, pieRectF);
                        else
                            g.FillPie(brush, pieRect, startAngle, Math.Max(sweep, 0.1f));
                    }

                    startAngle += sweep;
                }
            }

            var legendItems = slices.Count > 0 ? slices : BuildEmptyLegend().ToList();
            var legendX = x;
            var legendY = y + size + 24f;
            var swatch = 16f;

            foreach (var slice in legendItems)
            {
                using (var brush = new SolidBrush(slice.Color))
                    g.FillRectangle(brush, legendX, legendY + 2f, swatch, swatch);

                var percent = sliceTotal > 0 ? slice.Value / sliceTotal * 100m : 0m;
                var label = $"{slice.Label}  {FormatAmount(slice.Value)}  ({percent:0.0}%)";
                g.DrawString(label, legendFont, ink, legendX + swatch + 10f, legendY, nearFormat);
                legendY += 28f;
            }
        }

        private List<PieSlice> BuildSlices()
        {
            var slices = new List<PieSlice>();
            var salesCost = Math.Max(0m, _totalSalesCost);
            var expense = Math.Max(0m, _totalExpense);
            var netProfit = Math.Max(0m, _netProfit);

            if (salesCost > 0)
                slices.Add(new PieSlice("Sales Cost", salesCost, SalesCostColor));
            if (expense > 0)
                slices.Add(new PieSlice("Expense", expense, ExpenseColor));
            if (netProfit > 0)
                slices.Add(new PieSlice("Net Profit", netProfit, NetProfitColor));

            return slices;
        }

        private static IEnumerable<PieSlice> BuildEmptyLegend()
        {
            yield return new PieSlice("Sales Cost", 0m, SalesCostColor);
            yield return new PieSlice("Expense", 0m, ExpenseColor);
            yield return new PieSlice("Net Profit", 0m, NetProfitColor);
        }

        private static void DrawLabeledAmount(
            Graphics g,
            Font font,
            Brush ink,
            StringFormat nearFormat,
            StringFormat farFormat,
            string label,
            decimal amount,
            float labelLeft,
            float equalsX,
            float amountLeft,
            float amountRight,
            float y)
        {
            g.DrawString(label, font, ink, labelLeft, y, nearFormat);
            g.DrawString("=", font, ink, equalsX, y, nearFormat);

            var amountRect = new RectangleF(amountLeft, y, amountRight - amountLeft, font.Height + 4f);
            g.DrawString(FormatAmount(amount), font, ink, amountRect, farFormat);
        }

        private static void DrawNetResult(
            Graphics g,
            Font font,
            Brush ink,
            StringFormat nearFormat,
            StringFormat farFormat,
            decimal amount,
            string resultLabel,
            float amountLeft,
            float amountRight,
            float labelGap,
            float y)
        {
            var amountRect = new RectangleF(amountLeft, y, amountRight - amountLeft, font.Height + 4f);
            g.DrawString(FormatAmount(amount), font, ink, amountRect, farFormat);
            g.DrawString(resultLabel, font, ink, amountRight + labelGap, y, nearFormat);
        }

        private static void DrawAmountRule(Graphics g, Pen pen, float amountLeft, float amountRight, float y)
        {
            g.DrawLine(pen, amountLeft, y, amountRight, y);
        }

        private static string FormatAmount(decimal amount)
        {
            return amount.ToString("N2", CultureInfo.CurrentCulture) + "/=";
        }

        private static StringFormat CreateMeasureFormat()
        {
            return new StringFormat(StringFormat.GenericTypographic)
            {
                FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoClip
            };
        }

        private static StringFormat CreateNearFormat()
        {
            return new StringFormat(StringFormat.GenericTypographic)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                FormatFlags = StringFormatFlags.NoClip
            };
        }

        private static StringFormat CreateFarFormat()
        {
            return new StringFormat(StringFormat.GenericTypographic)
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Near,
                FormatFlags = StringFormatFlags.NoClip
            };
        }

        private static GraphicsPath CreateRoundedRectangle(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2f;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private sealed class PieSlice
        {
            public PieSlice(string label, decimal value, Color color)
            {
                Label = label;
                Value = value;
                Color = color;
            }

            public string Label { get; }
            public decimal Value { get; }
            public Color Color { get; }
        }
    }
}
