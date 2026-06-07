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
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            var qty = (int)numQuantity.Value;
            var error = _repository.AdjustStock(_product.Id, _type, qty, txtNotes.Text.Trim());

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
