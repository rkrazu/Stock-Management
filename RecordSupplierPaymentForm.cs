using System;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class RecordSupplierPaymentForm : Form
    {
        private readonly StockRepository _repository;
        private readonly Guid? _defaultSupplierId;

        public RecordSupplierPaymentForm(StockRepository repository, Supplier supplier = null)
        {
            _repository = repository;
            _defaultSupplierId = supplier?.Id;
            InitializeComponent();
            UiStyles.Apply(this);
            Text = "Record Supplier Payment";

            cmbMethod.Items.AddRange(new object[] { "Cash", "Bank", "Mobile Banking", "Cheque", "Other" });
            cmbMethod.SelectedIndex = 0;
            dtpPaidAt.Value = DateTime.Now;

            LoadSuppliers();
            if (_defaultSupplierId.HasValue)
                cmbSupplier.SelectedValue = _defaultSupplierId.Value;
        }

        private void LoadSuppliers()
        {
            cmbSupplier.DisplayMember = "Name";
            cmbSupplier.ValueMember = "Id";
            cmbSupplier.DataSource = _repository.SearchSuppliers(string.Empty).ToList();
        }

        private void CmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cmbSupplier.SelectedValue is Guid supplierId))
            {
                lblSupplierBalance.Text = string.Empty;
                return;
            }

            var balance = _repository.GetSupplierBalance(supplierId);
            lblSupplierBalance.Text = $"Due: {balance:C2}";
            if (balance > 0)
                numAmount.Value = Math.Min(numAmount.Maximum, Math.Max(numAmount.Minimum, balance));
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!(cmbSupplier.SelectedValue is Guid supplierId))
            {
                MessageBox.Show("Select a supplier.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var payment = new SupplierPayment
            {
                SupplierId = supplierId,
                Amount = numAmount.Value,
                PaymentMethod = cmbMethod.SelectedItem?.ToString() ?? "Cash",
                Reference = txtReference.Text.Trim(),
                Notes = txtNotes.Text.Trim(),
                PaidAt = dtpPaidAt.Value
            };

            var error = _repository.RecordSupplierPayment(payment);
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
