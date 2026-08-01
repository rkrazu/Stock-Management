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

            var payment = new SupplierPayment
            {
                Id = Guid.NewGuid(),
                SupplierId = supplierId,
                CashAccountId = method.CashAccountId,
                Amount = numAmount.Value,
                PaymentMethod = method.Name,
                Reference = txtReference.Text.Trim(),
                Notes = txtNotes.Text.Trim(),
                PaidAt = dtpPaidAt.Value
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
    }
}
