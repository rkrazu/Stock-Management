using System;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class CustomerEditForm : Form
    {
        private readonly StockRepository _repository;
        private readonly Customer _customer;
        private readonly bool _isNew;

        public CustomerEditForm(StockRepository repository, Customer customer = null)
        {
            _repository = repository;
            _isNew = customer == null;
            _customer = customer ?? new Customer();
            InitializeComponent();
            Text = _isNew ? "Add Customer" : "Edit Customer";
            btnSave.Text = _isNew ? "Add" : "Save";

            if (!_isNew)
            {
                txtName.Text = _customer.Name;
                txtAddress.Text = _customer.Address;
                txtPhone.Text = _customer.Phone;
                txtEmail.Text = _customer.Email;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var name = txtName.Text.Trim();
            var phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Customer name is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Phone number is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return;
            }

            if (_repository.PhoneExists(phone, _isNew ? (Guid?)null : _customer.Id))
            {
                MessageBox.Show("A customer with this phone number already exists.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return;
            }

            _customer.Name = name;
            _customer.Address = txtAddress.Text.Trim();
            _customer.Phone = phone;
            _customer.Email = txtEmail.Text.Trim();

            if (_isNew)
                _repository.AddCustomer(_customer);
            else
                _repository.UpdateCustomer(_customer);

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
