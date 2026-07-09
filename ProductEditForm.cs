using System;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class ProductEditForm : Form
    {
        private const int QuantityRowIndex = 4;
        private const int ReorderRowIndex = 5;
        private const float RowHeight = 42F;
        private const int FormWidth = 560;
        private readonly StockRepository _repository;
        private readonly Product _product;
        private readonly ProductType _productType;
        private readonly bool _isNew;

        public ProductEditForm(StockRepository repository, Product product = null, ProductType? inventoryContext = null)
        {
            _repository = repository;
            _isNew = product == null;
            _product = product ?? new Product();
            _productType = product?.ProductType ?? inventoryContext ?? ProductType.FG;
            InitializeComponent();
            UiStyles.Apply(this);
            Text = GetFormTitle();
            btnSave.Text = _isNew ? "Add" : "Save";

            if (!_isNew)
            {
                txtSku.Text = _product.Sku;
                txtName.Text = _product.Name;
                numPrice.Value = ClampDecimal(_product.UnitPrice, numPrice.Minimum, numPrice.Maximum);
                numReorder.Value = Math.Max(0, Math.Min(numReorder.Maximum, _product.ReorderLevel));
                if (_product.ProductType == ProductType.RawMaterial)
                    numQuantity.Value = Math.Max(0, Math.Min(numQuantity.Maximum, _product.Quantity));
            }

            ApplyProductTypeFields();

            if (!_isNew)
                SetCategoryValue(_product.Category);
        }

        private string GetFormTitle()
        {
            var typeLabel = _productType == ProductType.RawMaterial ? "Raw Material" : "FG";
            return _isNew ? $"Add {typeLabel} Product" : $"Edit {typeLabel} Product";
        }

        private void LoadCategories()
        {
            var current = cmbCategory.Text?.Trim() ?? string.Empty;
            cmbCategory.Items.Clear();
            foreach (var category in _repository.GetProductCategories(_productType))
                cmbCategory.Items.Add(category);

            if (!string.IsNullOrEmpty(current))
                SetCategoryValue(current);
        }

        private void SetCategoryValue(string category)
        {
            var value = category?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                cmbCategory.Text = string.Empty;
                return;
            }

            if (!cmbCategory.Items.Contains(value))
                cmbCategory.Items.Add(value);

            cmbCategory.Text = value;
        }

        private void ApplyProductTypeFields()
        {
            var isRawMaterial = _productType == ProductType.RawMaterial;
            lblPrice.Text = isRawMaterial ? "Unit cost:" : "Sale price:";

            var showQuantity = isRawMaterial && _isNew;
            lblQuantity.Visible = showQuantity;
            numQuantity.Visible = showQuantity;
            lblReorder.Visible = true;
            numReorder.Visible = true;
            tlpMain.RowStyles[QuantityRowIndex].Height = showQuantity ? RowHeight : 0F;
            tlpMain.RowStyles[ReorderRowIndex].Height = RowHeight;

            LoadCategories();
            ResizeToContent();
        }

        private void ResizeToContent()
        {
            tlpMain.PerformLayout();
            panelButtons.PerformLayout();
            ClientSize = new System.Drawing.Size(FormWidth, tlpMain.PreferredSize.Height + panelButtons.Height);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ResizeToContent();
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
            _product.Category = cmbCategory.Text.Trim();
            _product.ProductType = _productType;
            _product.UnitPrice = numPrice.Value;
            _product.ReorderLevel = (int)numReorder.Value;

            if (_isNew)
            {
                _product.Quantity = _productType == ProductType.RawMaterial
                    ? (int)numQuantity.Value
                    : 0;
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
