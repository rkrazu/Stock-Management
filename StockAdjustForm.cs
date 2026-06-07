using System;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class StockAdjustForm : Form
    {
        private readonly StockRepository _repository;
        private readonly Product _product;
        private readonly TransactionType _type;
        private Customer _selectedCustomer;

        public StockAdjustForm(StockRepository repository, Product product, TransactionType type)
        {
            _repository = repository;
            _product = product;
            _type = type;
            InitializeComponent();

            Text = type == TransactionType.StockIn ? "Stock In" : "Stock Out";
            lblProduct.Text = $"{product.Name} ({product.Sku})";
            lblAvailable.Text = type == TransactionType.StockOut
                ? $"Available: {product.Quantity}"
                : $"Current stock: {product.Quantity}";

            if (type == TransactionType.StockOut)
            {
                lblCustomer.Visible = true;
                txtCustomerSearch.Visible = true;
                lstCustomers.Visible = true;
                btnClearCustomer.Visible = true;

                lblQuantity.Location = new System.Drawing.Point(20, 230);
                numQuantity.Location = new System.Drawing.Point(120, 227);
                lblNotes.Location = new System.Drawing.Point(20, 265);
                txtNotes.Location = new System.Drawing.Point(20, 285);
                btnApply.Location = new System.Drawing.Point(224, 360);
                btnCancel.Location = new System.Drawing.Point(305, 360);
                ClientSize = new System.Drawing.Size(400, 405);

                lstCustomers.FormattingEnabled = true;
                lstCustomers.Format += LstCustomers_Format;
                txtCustomerSearch.TextChanged += (s, e) => RefreshCustomerList();
                lstCustomers.SelectedIndexChanged += LstCustomers_SelectedIndexChanged;
                btnClearCustomer.Click += (s, e) => ClearSelectedCustomer();
                RefreshCustomerList();
            }
        }

        private void RefreshCustomerList()
        {
            var term = txtCustomerSearch.Text;

            lstCustomers.BeginUpdate();
            lstCustomers.Items.Clear();
            _selectedCustomer = null;

            foreach (var c in _repository.SearchCustomers(term))
                lstCustomers.Items.Add(c);

            lstCustomers.ClearSelected();
            lstCustomers.EndUpdate();
        }

        private void LstCustomers_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is Customer c)
            {
                e.Value = string.IsNullOrWhiteSpace(c.Phone)
                    ? c.Name
                    : $"{c.Name} — {c.Phone}";
            }
        }

        private void LstCustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedCustomer = lstCustomers.SelectedItem as Customer;
        }

        private void ClearSelectedCustomer()
        {
            _selectedCustomer = null;
            lstCustomers.ClearSelected();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            var qty = (int)numQuantity.Value;
            Guid? customerId = _type == TransactionType.StockOut ? _selectedCustomer?.Id : null;
            var error = _repository.AdjustStock(_product.Id, _type, qty, txtNotes.Text.Trim(), customerId);

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
