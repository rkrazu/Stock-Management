using System;
using System.Linq;
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
            ConfigurePaymentPanel();
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
            panelPayment.Visible = false;
            RenderInvoice();
        }

        private void ConfigurePaymentPanel()
        {
            panelPayment.Visible = true;

            var accounts = _repository.GetCashAndBankAccounts().ToList();
            cmbCashAccount.DisplayMember = "Name";
            cmbCashAccount.ValueMember = "Id";
            cmbCashAccount.DataSource = accounts;
            if (accounts.Count > 0)
                cmbCashAccount.SelectedIndex = 0;

            numAmountPaid.Maximum = Math.Max(0, _invoice.TotalAmount);
            numAmountPaid.Value = Math.Min(numAmountPaid.Maximum, Math.Max(0, _stockOutRequest.AmountPaidAtSale));
            UpdatePaymentSummary();

            numAmountPaid.ValueChanged += (s, e) => UpdatePaymentSummary();
        }

        private void UpdatePaymentSummary()
        {
            var total = _invoice.TotalAmount;
            var paid = numAmountPaid.Value;
            if (paid > total)
            {
                paid = total;
                numAmountPaid.Value = paid;
            }

            lblPaymentTotal.Text = total.ToString("C2");
            lblBalanceDue.Text = Math.Max(0, total - paid).ToString("C2");
            cmbCashAccount.Enabled = paid > 0;
        }

        private void ApplyPaymentToRequest()
        {
            if (!_isDraft || _stockOutRequest == null)
                return;

            _stockOutRequest.AmountPaidAtSale = numAmountPaid.Value;
            _stockOutRequest.CashAccountId = numAmountPaid.Value > 0 && cmbCashAccount.SelectedValue is Guid accountId
                ? accountId
                : (Guid?)null;
            _invoice.AmountPaid = _stockOutRequest.AmountPaidAtSale;
        }

        private void RenderInvoice()
        {
            invoicePreview.SetInvoice(_invoice, _isDraft);
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            ApplyPaymentToRequest();

            var validationError = _repository.ValidateStockOut(_stockOutRequest);
            if (validationError != null)
            {
                MessageBox.Show(validationError, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _invoice = _repository.BuildStockOutInvoicePreview(_stockOutRequest);
                RenderInvoice();
                return;
            }

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
            if (_isDraft)
                ApplyPaymentToRequest();

            InvoiceDocumentBuilder.Print(_invoice, _isDraft);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            DialogResult = _isDraft ? DialogResult.Cancel : DialogResult.OK;
            Close();
        }
    }
}
