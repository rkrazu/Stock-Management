using System;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class SupplierEditForm : Form
    {
        private readonly StockRepository _repository;
        private readonly Supplier _supplier;
        private readonly bool _isNew;

        public SupplierEditForm(StockRepository repository, Supplier supplier = null)
        {
            _repository = repository;
            _isNew = supplier == null;
            _supplier = supplier ?? new Supplier();
            InitializeComponent();
            UiStyles.Apply(this);
            Text = _isNew ? "Add Supplier" : "Edit Supplier";
            btnSave.Text = _isNew ? "Add" : "Save";

            if (!_isNew)
            {
                txtName.Text = _supplier.Name;
                txtAddress.Text = _supplier.Address;
                txtPhone.Text = _supplier.Phone;
                txtEmail.Text = _supplier.Email;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var name = txtName.Text.Trim();
            var phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Supplier name is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Phone number is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return;
            }

            if (_repository.SupplierPhoneExists(phone, _isNew ? (Guid?)null : _supplier.Id))
            {
                MessageBox.Show("A supplier with this phone number already exists.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return;
            }

            _supplier.Name = name;
            _supplier.Address = txtAddress.Text.Trim();
            _supplier.Phone = phone;
            _supplier.Email = txtEmail.Text.Trim();

            if (_isNew)
                _repository.AddSupplier(_supplier);
            else
                _repository.UpdateSupplier(_supplier);

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
