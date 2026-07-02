using System;
using System.Drawing;
using System.Windows.Forms;
using Stock_Managemnet.Models;

namespace Stock_Managemnet.Controls
{
    public class InvoicePreviewControl : Panel
    {
        private const int DocumentWidth = 820;
        private const int VerticalPadding = 20;
        private const int HorizontalPadding = 24;

        private readonly Panel _documentSurface = new Panel();
        private Invoice _invoice;
        private bool _isDraft;

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

        public void SetInvoice(Invoice invoice, bool isDraft)
        {
            _invoice = invoice;
            _isDraft = isDraft;
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

            e.Graphics.Clear(Color.White);
            InvoiceDocumentBuilder.Paint(
                e.Graphics,
                new Rectangle(0, 0, _documentSurface.Width, _documentSurface.Height),
                _invoice,
                _isDraft,
                InvoiceDocumentBuilder.InvoiceRenderProfile.Screen);
        }

        private void LayoutDocument()
        {
            if (_invoice == null)
                return;

            var documentHeight = InvoiceDocumentBuilder.MeasureHeight(
                _invoice,
                DocumentWidth,
                InvoiceDocumentBuilder.InvoiceRenderProfile.Screen);

            _documentSurface.Size = new Size(DocumentWidth, documentHeight);

            var scrollWidth = Math.Max(ClientSize.Width, DocumentWidth + (HorizontalPadding * 2));
            var scrollHeight = documentHeight + (VerticalPadding * 2);
            AutoScrollMinSize = new Size(scrollWidth, scrollHeight);

            _documentSurface.Left = Math.Max(HorizontalPadding, (scrollWidth - DocumentWidth) / 2);
            _documentSurface.Top = VerticalPadding;
        }
    }
}
