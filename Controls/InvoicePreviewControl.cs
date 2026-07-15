using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet.Controls
{
    public class InvoicePreviewControl : Panel
    {
        private const int VerticalPadding = 12;
        private const int HorizontalPadding = 12;

        private readonly Panel _documentSurface = new Panel();
        private Invoice _invoice;
        private bool _isDraft;
        private InvoiceDocumentBuilder.DocumentKind _kind = InvoiceDocumentBuilder.DocumentKind.Invoice;

        public InvoicePreviewControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            BackColor = Color.FromArgb(245, 245, 245);
            AutoScroll = true;

            _documentSurface.BackColor = Color.White;
            _documentSurface.BorderStyle = BorderStyle.FixedSingle;
            _documentSurface.TabStop = false;
            _documentSurface.Paint += DocumentSurface_Paint;
            Controls.Add(_documentSurface);
        }

        public void SetInvoice(
            Invoice invoice,
            bool isDraft,
            InvoiceDocumentBuilder.DocumentKind kind = InvoiceDocumentBuilder.DocumentKind.Invoice)
        {
            _invoice = invoice;
            _isDraft = isDraft;
            _kind = kind;
            LayoutDocument();
            _documentSurface.Invalidate();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
                LayoutDocument();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutDocument();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            base.OnPaint(e);
        }

        private void DocumentSurface_Paint(object sender, PaintEventArgs e)
        {
            if (_invoice == null)
                return;

            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            InvoiceDocumentBuilder.Paint(
                graphics,
                new Rectangle(0, 0, _documentSurface.Width, _documentSurface.Height),
                _invoice,
                _isDraft,
                InvoiceDocumentBuilder.InvoiceRenderProfile.Screen,
                _kind);
        }

        private void LayoutDocument()
        {
            if (_invoice == null)
                return;

            var availableWidth = Math.Max(320, ClientSize.Width - (HorizontalPadding * 2));
            var pageSize = InvoicePaperAssets.GetLogicalPageSize(availableWidth);

            _documentSurface.Size = pageSize;
            _documentSurface.Left = Math.Max(0, (ClientSize.Width - pageSize.Width) / 2);
            _documentSurface.Top = VerticalPadding;

            AutoScrollMinSize = new Size(
                pageSize.Width + (HorizontalPadding * 2),
                pageSize.Height + (VerticalPadding * 2));
        }
    }
}
