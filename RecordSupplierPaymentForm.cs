using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class RecordSupplierPaymentForm : Form
    {
        private static readonly Guid AgainstDueId = new Guid("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");

        private readonly StockRepository _repository;
        private readonly Guid? _defaultSupplierId;
        private string _receiptSourcePath;

        public RecordSupplierPaymentForm(StockRepository repository, Supplier supplier = null)
        {
            _repository = repository;
            _defaultSupplierId = supplier?.Id;
            InitializeComponent();
            UiStyles.Apply(this);
            Text = "Record Supplier Payment";

            dtpPaidAt.Value = DateTime.Now;
            LoadPaymentMethods();
            LoadSuppliers();
            UpdateReceiptLabel();
            if (_defaultSupplierId.HasValue)
                cmbSupplier.SelectedValue = _defaultSupplierId.Value;
            else
                LoadAgainstOptions();
        }

        private void LoadPaymentMethods()
        {
            var methods = _repository.GetPaymentMethodOptions().ToList();
            cmbMethod.DisplayMember = "Name";
            cmbMethod.ValueMember = "CashAccountId";
            cmbMethod.DataSource = methods;
            if (methods.Count > 0)
                cmbMethod.SelectedIndex = 0;
        }

        private void LoadSuppliers()
        {
            cmbSupplier.DisplayMember = "Name";
            cmbSupplier.ValueMember = "Id";
            cmbSupplier.DataSource = _repository.SearchSuppliers(string.Empty).ToList();
        }

        private void LoadAgainstOptions()
        {
            var items = new List<AgainstComboItem>
            {
                new AgainstComboItem { Id = Guid.Empty, Display = "Advance Payment" }
            };

            if (cmbSupplier.SelectedValue is Guid supplierId)
            {
                var balance = _repository.GetSupplierBalance(supplierId);
                if (balance > 0)
                    items.Add(new AgainstComboItem { Id = AgainstDueId, Display = $"Against due  |  {balance:C2}" });
            }

            cmbAgainst.DisplayMember = "Display";
            cmbAgainst.ValueMember = "Id";
            cmbAgainst.DataSource = items;
            cmbAgainst.SelectedIndex = 0;
        }

        private void CmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cmbSupplier.SelectedValue is Guid supplierId))
            {
                lblSupplierBalance.Text = string.Empty;
                LoadAgainstOptions();
                return;
            }

            var balance = _repository.GetSupplierBalance(supplierId);
            lblSupplierBalance.Text = balance >= 0
                ? $"Due: {balance:C2}"
                : $"Advance / credit: {Math.Abs(balance):C2}";

            LoadAgainstOptions();

            if (balance > 0 && cmbAgainst.Items.Count > 1)
            {
                cmbAgainst.SelectedIndex = 1;
                numAmount.Value = Math.Min(numAmount.Maximum, Math.Max(numAmount.Minimum, balance));
            }
            else
            {
                numAmount.Value = Math.Max(numAmount.Minimum, 0.01m);
            }
        }

        private void CmbAgainst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cmbSupplier.SelectedValue is Guid supplierId))
                return;

            var balance = _repository.GetSupplierBalance(supplierId);
            var isAgainstDue = cmbAgainst.SelectedValue is Guid id && id == AgainstDueId;
            if (isAgainstDue && balance > 0)
                numAmount.Value = Math.Min(numAmount.Maximum, Math.Max(numAmount.Minimum, balance));
        }

        private void BtnBrowseReceipt_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Select money receipt photo",
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif;*.tiff|All files|*.*",
                CheckFileExists = true,
                Multiselect = false
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                if (!ReceiptStorageService.IsAllowedImage(dialog.FileName))
                {
                    MessageBox.Show("Select a photo file (JPG, PNG, BMP, GIF, or TIFF).", Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _receiptSourcePath = dialog.FileName;
                UpdateReceiptLabel();
            }
        }

        private void BtnClearReceipt_Click(object sender, EventArgs e)
        {
            _receiptSourcePath = null;
            UpdateReceiptLabel();
        }

        private void UpdateReceiptLabel()
        {
            if (string.IsNullOrWhiteSpace(_receiptSourcePath))
            {
                lblReceiptFile.Text = "No photo selected (optional)";
                btnClearReceipt.Enabled = false;
            }
            else
            {
                lblReceiptFile.Text = System.IO.Path.GetFileName(_receiptSourcePath);
                btnClearReceipt.Enabled = true;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!(cmbSupplier.SelectedValue is Guid supplierId))
            {
                MessageBox.Show("Select a supplier.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(cmbMethod.SelectedItem is PaymentMethodOption method) || method.CashAccountId == Guid.Empty)
            {
                MessageBox.Show("Select a payment method.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var isAdvance = !(cmbAgainst.SelectedValue is Guid againstId) || againstId == Guid.Empty;

            var payment = new SupplierPayment
            {
                Id = Guid.NewGuid(),
                SupplierId = supplierId,
                CashAccountId = method.CashAccountId,
                Amount = numAmount.Value,
                PaymentMethod = method.Name,
                Reference = txtReference.Text.Trim(),
                Notes = txtNotes.Text.Trim(),
                PaidAt = dtpPaidAt.Value,
                IsAdvance = isAdvance
            };

            if (!string.IsNullOrWhiteSpace(_receiptSourcePath))
            {
                var receiptError = _repository.AttachReceiptToSupplierPayment(payment, _receiptSourcePath);
                if (receiptError != null)
                {
                    MessageBox.Show(receiptError, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

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

        private sealed class AgainstComboItem
        {
            public Guid Id { get; set; }
            public string Display { get; set; }
        }
    }
}
