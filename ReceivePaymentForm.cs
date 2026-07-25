using System;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class ReceivePaymentForm : Form
    {
        private readonly StockRepository _repository;
        private readonly Guid? _defaultCustomerId;
        private readonly Guid? _defaultInvoiceId;

        public ReceivePaymentForm(StockRepository repository, Customer customer = null, Invoice invoice = null)
        {
            _repository = repository;
            _defaultCustomerId = customer?.Id ?? invoice?.CustomerId;
            _defaultInvoiceId = invoice?.Id;
            InitializeComponent();
            UiStyles.Apply(this);
            Text = "Receive Payment";

            cmbMethod.Items.AddRange(new object[] { "Cash", "Bank", "Mobile Banking", "Cheque", "Other" });
            cmbMethod.SelectedIndex = 0;
            dtpPaidAt.Value = DateTime.Now;

            numAmount.Minimum = 0m;
            numAmount.Maximum = 99999999m;
            numAmount.Value = 0m;

            LoadCashAccounts();
            LoadCustomers();

            if (_defaultCustomerId.HasValue)
                cmbCustomer.SelectedValue = _defaultCustomerId.Value;

            if (_defaultInvoiceId.HasValue && cmbInvoice.Enabled)
                cmbInvoice.SelectedValue = _defaultInvoiceId.Value;

            numAmount.Value = 0m;
        }

        private void LoadCashAccounts()
        {
            var accounts = _repository.GetCashAndBankAccounts().ToList();
            cmbCashAccount.DisplayMember = "Name";
            cmbCashAccount.ValueMember = "Id";
            cmbCashAccount.DataSource = accounts;
            if (accounts.Count > 0)
                cmbCashAccount.SelectedIndex = 0;
        }

        private void LoadCustomers()
        {
            cmbCustomer.DisplayMember = "Name";
            cmbCustomer.ValueMember = "Id";
            cmbCustomer.DataSource = _repository.SearchCustomers(string.Empty).ToList();
        }

        private void LoadInvoices()
        {
            if (!(cmbCustomer.SelectedValue is Guid customerId))
            {
                cmbInvoice.DataSource = null;
                cmbInvoice.Enabled = false;
                return;
            }

            var invoices = _repository.GetOpenInvoices(customerId)
                .Select(i => new
                {
                    i.Id,
                    Display = $"{i.InvoiceNumber}  |  Due {i.BalanceDue:C2}"
                })
                .ToList();

            cmbInvoice.DisplayMember = "Display";
            cmbInvoice.ValueMember = "Id";
            cmbInvoice.DataSource = invoices;
            cmbInvoice.Enabled = invoices.Count > 0;

            if (_defaultInvoiceId.HasValue && invoices.Any(i => i.Id == _defaultInvoiceId.Value))
                cmbInvoice.SelectedValue = _defaultInvoiceId.Value;
            else if (invoices.Count > 0)
                cmbInvoice.SelectedIndex = 0;
        }

        private void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedValue is Guid customerId)
            {
                var balance = _repository.GetCustomerBalance(customerId);
                lblCustomerBalance.Text = $"Outstanding balance: {balance:C2}";
            }
            else
            {
                lblCustomerBalance.Text = string.Empty;
            }

            LoadInvoices();
            numAmount.Value = 0m;
        }

        private void CmbInvoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            numAmount.Value = 0m;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!(cmbCustomer.SelectedValue is Guid customerId))
            {
                MessageBox.Show("Select a customer.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(cmbCashAccount.SelectedValue is Guid cashAccountId))
            {
                MessageBox.Show("Select a cash or bank account.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (numAmount.Value <= 0m)
            {
                MessageBox.Show("Please add amount.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numAmount.Focus();
                return;
            }

            Guid? invoiceId = null;
            if (cmbInvoice.Enabled && cmbInvoice.SelectedValue is Guid selectedInvoiceId)
                invoiceId = selectedInvoiceId;

            var payment = new CustomerPayment
            {
                CustomerId = customerId,
                InvoiceId = invoiceId,
                CashAccountId = cashAccountId,
                Amount = numAmount.Value,
                PaymentMethod = cmbMethod.SelectedItem?.ToString(),
                Reference = txtReference.Text.Trim(),
                Notes = txtNotes.Text.Trim(),
                PaidAt = dtpPaidAt.Value
            };

            var error = _repository.ReceivePayment(payment);
            if (error != null)
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
