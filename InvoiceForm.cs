using System;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class InvoiceForm : Form
    {
        private readonly StockRepository _repository;
        private readonly StockOutRequest _stockOutRequest;
        private Invoice _invoice;
        private readonly bool _isDraft;

        public InvoiceForm(StockRepository repository, StockOutRequest stockOutRequest)
        {
            _repository = repository;
            _stockOutRequest = stockOutRequest;
            _isDraft = true;
            _invoice = _repository.BuildStockOutInvoicePreview(stockOutRequest);
            InitializeComponent();
            UiStyles.Apply(this);
            Text = "Stock Out Invoice";
            btnSubmit.Visible = true;
            btnSubmit.Text = "Submit Invoice";
            btnClose.Text = "Cancel";
            btnPrint.Visible = true;
            RenderInvoice();
        }

        public InvoiceForm(StockRepository repository, Invoice invoice)
        {
            _repository = repository;
            _invoice = invoice;
            _isDraft = false;
            InitializeComponent();
            UiStyles.Apply(this);
            Text = $"Invoice {invoice.InvoiceNumber}";
            btnSubmit.Visible = false;
            btnClose.Text = "Close";
            RenderInvoice();
        }

        private void RenderInvoice()
        {
            invoicePreview.SetInvoice(_invoice, _isDraft);
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            var error = _repository.CompleteStockOut(_stockOutRequest);
            if (error != null)
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _invoice = _repository.BuildStockOutInvoicePreview(_stockOutRequest);
                RenderInvoice();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            InvoiceDocumentBuilder.Print(_invoice, _isDraft);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            DialogResult = _isDraft ? DialogResult.Cancel : DialogResult.OK;
            Close();
        }
    }
}
