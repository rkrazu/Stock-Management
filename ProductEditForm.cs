using System;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class ProductEditForm : Form
    {
        private readonly StockRepository _repository;
        private readonly Product _product;
        private readonly bool _isNew;

        public ProductEditForm(StockRepository repository, Product product = null)
        {
            _repository = repository;
            _isNew = product == null;
            _product = product ?? new Product();
            InitializeComponent();
            UiStyles.Apply(this);
            Text = _isNew ? "Add Product" : "Edit Product";
            btnSave.Text = _isNew ? "Add" : "Save";

            if (!_isNew)
            {
                txtSku.Text = _product.Sku;
                txtName.Text = _product.Name;
                txtCategory.Text = _product.Category;
                numPrice.Value = ClampDecimal(_product.UnitPrice, numPrice.Minimum, numPrice.Maximum);
                numReorder.Value = Math.Max(0, Math.Min(numReorder.Maximum, _product.ReorderLevel));
                numQuantity.Value = Math.Max(0, Math.Min(numQuantity.Maximum, _product.Quantity));
                numQuantity.Enabled = _isNew;
            }
        }

        private static decimal ClampDecimal(decimal value, decimal min, decimal max) =>
            Math.Max(min, Math.Min(max, value));

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var sku = txtSku.Text.Trim();
            var name = txtName.Text.Trim();

            if (string.IsNullOrEmpty(sku))
            {
                MessageBox.Show("SKU is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSku.Focus();
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Product name is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (_repository.SkuExists(sku, _isNew ? (Guid?)null : _product.Id))
            {
                MessageBox.Show("A product with this SKU already exists.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSku.Focus();
                return;
            }

            _product.Sku = sku;
            _product.Name = name;
            _product.Category = txtCategory.Text.Trim();
            _product.UnitPrice = numPrice.Value;
            _product.ReorderLevel = (int)numReorder.Value;

            if (_isNew)
            {
                _product.Quantity = (int)numQuantity.Value;
                _repository.AddProduct(_product);
            }
            else
            {
                _repository.UpdateProduct(_product);
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
