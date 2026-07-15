using System;
using System.Windows.Forms;
using Stock_Managemnet.Models;

namespace Stock_Managemnet
{
    public partial class ChalanForm : Form
    {
        private readonly Invoice _invoice;

        public ChalanForm(Invoice invoice)
        {
            _invoice = invoice ?? throw new ArgumentNullException(nameof(invoice));
            InitializeComponent();
            UiStyles.Apply(this);
            Text = $"Chalan {_invoice.InvoiceNumber}";
            invoicePreview.SetInvoice(_invoice, isDraft: false, InvoiceDocumentBuilder.DocumentKind.Chalan);
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            InvoiceDocumentBuilder.Print(_invoice, isDraft: false, InvoiceDocumentBuilder.DocumentKind.Chalan);
        }

        private void BtnPdf_Click(object sender, EventArgs e)
        {
            InvoiceDocumentBuilder.ExportPdf(_invoice, isDraft: false, this, InvoiceDocumentBuilder.DocumentKind.Chalan);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
