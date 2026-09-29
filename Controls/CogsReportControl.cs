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
    public sealed class CogsReportControl : ScrollableControl
    {
        private static readonly Color SalesCostColor = Color.FromArgb(59, 130, 246);
        private static readonly Color ExpenseColor = Color.FromArgb(239, 68, 68);
        private static readonly Color NetProfitColor = Color.FromArgb(34, 197, 94);
        private static readonly Color RawStockColor = Color.FromArgb(245, 158, 11);
        private static readonly Color FgStockColor = Color.FromArgb(16, 185, 129);
        private static readonly Color CustomerDueColor = Color.FromArgb(99, 102, 241);
        private static readonly Color SupplierPayableColor = Color.FromArgb(244, 63, 94);
        private static readonly Color EmptyPieColor = Color.FromArgb(229, 231, 235);

        private decimal _totalSales;
        private decimal _totalSalesCost;
        private decimal _totalProfit;
        private decimal _totalExpense;
        private decimal _netProfit;
        private decimal _rawStockValue;
        private decimal _fgStockValue;
        private decimal _customerDue;
        private decimal _supplierPayable;

        public CogsReportControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);
            AutoScroll = true;
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

        public void SetInvestment(decimal rawStockValue, decimal fgStockValue, decimal customerDue, decimal supplierPayable)
        {
            _rawStockValue = rawStockValue;
            _fgStockValue = fgStockValue;
            _customerDue = customerDue;
            _supplierPayable = supplierPayable;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var titleFont = new Font(Font.FontFamily, 22F, FontStyle.Bold))
            using (var bodyFont = new Font(Font.FontFamily, 15F))
            using (var totalFont = new Font(Font.FontFamily, 15F, FontStyle.Bold))
            using (var legendFont = new Font(Font.FontFamily, 12.5F))
            using (var ink = new SolidBrush(Color.Black))
            using (var titleFill = new SolidBrush(BackColor))
            using (var borderPen = new Pen(Color.Black, 2F))
            using (var rulePen = new Pen(Color.Black, 1.5F))
            using (var measureFormat = CreateMeasureFormat())
            using (var nearFormat = CreateNearFormat())
            using (var farFormat = CreateFarFormat())
            {
                const float boxWidth = 980f;
                const float padLeft = 36f;
                const float padRight = 28f;
                const float padBottom = 28f;
                const float contentTop = 36f;
                const float pieSize = 210f;
                const float pieColumnWidth = 340f;
                const float columnGap = 28f;
                const float cardGap = 40f;
                const float pieLegendGap = 24f;
                const float legendLineHeight = 28f;
                const float lineHeight = 38f;

                var titleSize = g.MeasureString("Investment", titleFont, int.MaxValue, measureFormat);
                var resultLabel = _netProfit < 0 ? "Net Loss" : "Net Profit";
                var titleHang = titleSize.Height / 2f;
                var cogsPieHeight = pieSize + pieLegendGap + legendLineHeight * 3f;
                var investmentPieHeight = pieSize + pieLegendGap + legendLineHeight * 4f;
                var cogsCalcHeight = 34f + lineHeight * 4f + 54f;
                var investmentCalcHeight = 28f + lineHeight * 4f + 36f;
                var cogsBoxHeight = contentTop + Math.Max(cogsCalcHeight, cogsPieHeight) + padBottom;
                var investmentBoxHeight = contentTop + Math.Max(investmentCalcHeight, investmentPieHeight) + padBottom;
                var compositionHeight = titleHang + cogsBoxHeight + cardGap + investmentBoxHeight;

                var originX = Math.Max(16f, (ClientSize.Width - boxWidth) / 2f);
                var originY = Math.Max(16f, (ClientSize.Height - compositionHeight) / 2f);

                var cogsBox = new RectangleF(originX, originY + titleHang, boxWidth, cogsBoxHeight);
                DrawTitledCard(g, titleFont, ink, titleFill, borderPen, measureFormat, nearFormat, cogsBox, "COGS");

                var calcRight = cogsBox.Right - padRight - pieColumnWidth - columnGap;
                var pieX = cogsBox.Right - padRight - pieColumnWidth;
                var pieY = cogsBox.Y + contentTop;
                var labelLeft = cogsBox.X + padLeft;
                var labelColumn = MeasureLabelColumn(
                    g, totalFont, measureFormat,
                    resultLabel, "Total Sales", "Total Sales Cost (-)", "Total Profit", "Total Expense (-)",
                    "Total Raw Stock Value", "Total FG Stock Value", "Total Customer Due Value",
                    "Payable to Supplier (-)", "Total Investment");
                var equalsX = labelLeft + labelColumn + 18f;
                var amountLeft = equalsX + 22f;
                var amountRight = calcRight;
                var y = cogsBox.Y + contentTop + 8f;

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
                DrawLabeledAmount(g, totalFont, ink, nearFormat, farFormat, resultLabel, _netProfit, labelLeft, equalsX, amountLeft, amountRight, y);

                DrawPieChart(g, legendFont, ink, nearFormat, pieX, pieY, pieSize, BuildSlices(), BuildEmptyLegend());

                var investmentBox = new RectangleF(originX, cogsBox.Bottom + cardGap, boxWidth, investmentBoxHeight);
                DrawTitledCard(g, titleFont, ink, titleFill, borderPen, measureFormat, nearFormat, investmentBox, "Investment");
                DrawInvestmentAmounts(
                    g, bodyFont, totalFont, ink, rulePen, nearFormat, farFormat,
                    investmentBox, labelLeft, equalsX, amountLeft, amountRight);
                DrawPieChart(
                    g, legendFont, ink, nearFormat,
                    pieX, investmentBox.Y + contentTop, pieSize,
                    BuildInvestmentSlices(),
                    BuildEmptyInvestmentLegend());

                var scrollSize = new Size(
                    (int)Math.Ceiling(boxWidth + 32f),
                    (int)Math.Ceiling(compositionHeight + 32f));
                if (AutoScrollMinSize != scrollSize)
                    AutoScrollMinSize = scrollSize;
            }
        }

        private static void DrawTitledCard(
            Graphics g,
            Font titleFont,
            Brush ink,
            Brush titleFill,
            Pen borderPen,
            StringFormat measureFormat,
            StringFormat nearFormat,
            RectangleF box,
            string title)
        {
            using (var path = CreateRoundedRectangle(box, 30f))
                g.DrawPath(borderPen, path);

            var titleSize = g.MeasureString(title, titleFont, int.MaxValue, measureFormat);
            var titleX = box.X + (box.Width - titleSize.Width) / 2f;
            var titleY = box.Y - titleSize.Height / 2f;
            var wipe = new RectangleF(titleX - 12f, titleY, titleSize.Width + 24f, titleSize.Height);
            g.FillRectangle(titleFill, wipe);
            g.DrawString(title, titleFont, ink, titleX, titleY, nearFormat);
        }

        private void DrawInvestmentAmounts(
            Graphics g,
            Font bodyFont,
            Font totalFont,
            Brush ink,
            Pen rulePen,
            StringFormat nearFormat,
            StringFormat farFormat,
            RectangleF box,
            float labelLeft,
            float equalsX,
            float amountLeft,
            float amountRight)
        {
            var lineHeight = 38f;
            var y = box.Y + 44f;

            DrawLabeledAmount(g, bodyFont, ink, nearFormat, farFormat, "Total Raw Stock Value", _rawStockValue, labelLeft, equalsX, amountLeft, amountRight, y);
            y += lineHeight;
            DrawLabeledAmount(g, bodyFont, ink, nearFormat, farFormat, "Total FG Stock Value", _fgStockValue, labelLeft, equalsX, amountLeft, amountRight, y);
            y += lineHeight;
            DrawLabeledAmount(g, bodyFont, ink, nearFormat, farFormat, "Total Customer Due Value", _customerDue, labelLeft, equalsX, amountLeft, amountRight, y);
            y += lineHeight;
            DrawLabeledAmount(g, bodyFont, ink, nearFormat, farFormat, "Payable to Supplier (-)", _supplierPayable, labelLeft, equalsX, amountLeft, amountRight, y);
            y += lineHeight + 4f;
            DrawAmountRule(g, rulePen, amountLeft, amountRight, y);
            y += 16f;
            DrawLabeledAmount(
                g, totalFont, ink, nearFormat, farFormat,
                "Total Investment",
                _rawStockValue + _fgStockValue + _customerDue - _supplierPayable,
                labelLeft, equalsX, amountLeft, amountRight, y);
        }

        private static float MeasureLabelColumn(
            Graphics g,
            Font font,
            StringFormat measureFormat,
            params string[] labels)
        {
            var width = 0f;
            foreach (var label in labels)
                width = Math.Max(width, g.MeasureString(label, font, int.MaxValue, measureFormat).Width);
            return width;
        }

        private static void DrawPieChart(
            Graphics g,
            Font legendFont,
            Brush ink,
            StringFormat nearFormat,
            float x,
            float y,
            float size,
            IList<PieSlice> slices,
            IEnumerable<PieSlice> emptyLegend)
        {
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

            var legendItems = slices.Count > 0 ? slices : emptyLegend.ToList();
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

        private List<PieSlice> BuildInvestmentSlices()
        {
            var slices = new List<PieSlice>();
            var raw = Math.Max(0m, _rawStockValue);
            var fg = Math.Max(0m, _fgStockValue);
            var due = Math.Max(0m, _customerDue);
            var payable = Math.Max(0m, _supplierPayable);

            if (raw > 0)
                slices.Add(new PieSlice("Raw Stock", raw, RawStockColor));
            if (fg > 0)
                slices.Add(new PieSlice("FG Stock", fg, FgStockColor));
            if (due > 0)
                slices.Add(new PieSlice("Customer Due", due, CustomerDueColor));
            if (payable > 0)
                slices.Add(new PieSlice("Supplier Payable", payable, SupplierPayableColor));

            return slices;
        }

        private static IEnumerable<PieSlice> BuildEmptyInvestmentLegend()
        {
            yield return new PieSlice("Raw Stock", 0m, RawStockColor);
            yield return new PieSlice("FG Stock", 0m, FgStockColor);
            yield return new PieSlice("Customer Due", 0m, CustomerDueColor);
            yield return new PieSlice("Supplier Payable", 0m, SupplierPayableColor);
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
