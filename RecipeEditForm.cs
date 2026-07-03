using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Controls;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class RecipeEditForm : Form
    {
        private readonly StockRepository _repository;
        private readonly ProductionRecipe _recipe;
        private readonly bool _isNew;
        private readonly List<ProductionMaterial> _materials = new List<ProductionMaterial>();

        public RecipeEditForm(StockRepository repository, ProductionRecipe recipe = null)
        {
            _repository = repository;
            _isNew = recipe == null;
            _recipe = recipe ?? new ProductionRecipe();
            InitializeComponent();
            UiStyles.Apply(this);

            Text = _isNew ? "Add Production Recipe" : "Edit Production Recipe";
            btnSave.Text = _isNew ? "Add" : "Save";

            ConfigureProductSelectors();
            ConfigureMaterialGrid();
            GridExportUi.Enable(dgvMaterials, "Recipe Materials");

            if (!_isNew)
            {
                txtName.Text = _recipe.Name;
                SetSelectedOutputProduct(_recipe.OutputProductId);
                _materials.AddRange(_recipe.Materials.Select(m => new ProductionMaterial
                {
                    ProductId = m.ProductId,
                    ProductSku = m.ProductSku,
                    ProductName = m.ProductName,
                    QuantityPerUnit = m.QuantityPerUnit
                }));
                RefreshMaterialGrid();
            }

            Shown += RecipeEditForm_Shown;
        }

        private void RecipeEditForm_Shown(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                outputProductSelect.HideDropDownIfOpen();
                materialProductSelect.HideDropDownIfOpen();
                ActiveControl = null;
            }));
        }

        private void ConfigureProductSelectors()
        {
            outputProductSelect.BindSearch(
                term => _repository.SearchProducts(term),
                (product, term) => _repository.ProductMatchesSearchTerm(product, term));

            materialProductSelect.BindSearch(
                term => _repository.SearchProducts(term),
                (product, term) => _repository.ProductMatchesSearchTerm(product, term));
        }

        private void SetSelectedOutputProduct(Guid productId)
        {
            var product = _repository.GetProduct(productId);
            if (product != null)
                outputProductSelect.SelectProduct(product);
        }

        private void ConfigureMaterialGrid()
        {
            dgvMaterials.AutoGenerateColumns = false;
            dgvMaterials.Columns.Clear();
            dgvMaterials.Columns.Add("Sku", "SKU");
            dgvMaterials.Columns.Add("Name", "Material");
            dgvMaterials.Columns.Add("QuantityPerUnit", "Qty / Unit");
            dgvMaterials.Columns["QuantityPerUnit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void RefreshMaterialGrid()
        {
            dgvMaterials.Rows.Clear();
            foreach (var material in _materials)
            {
                var idx = dgvMaterials.Rows.Add(material.ProductSku, material.ProductName, material.QuantityPerUnit);
                dgvMaterials.Rows[idx].Tag = material;
            }
        }

        private void BtnAddMaterial_Click(object sender, EventArgs e)
        {
            var product = materialProductSelect.SelectedProduct;
            if (product == null)
            {
                MessageBox.Show("Select a material product.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var qty = (int)numMaterialQty.Value;
            if (qty <= 0)
            {
                MessageBox.Show("Material quantity must be greater than zero.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existing = _materials.FirstOrDefault(m => m.ProductId == product.Id);
            if (existing != null)
            {
                existing.QuantityPerUnit = qty;
            }
            else
            {
                _materials.Add(new ProductionMaterial
                {
                    ProductId = product.Id,
                    ProductSku = product.Sku,
                    ProductName = product.Name,
                    QuantityPerUnit = qty
                });
            }

            RefreshMaterialGrid();
            materialProductSelect.ClearSelection();
        }

        private void BtnRemoveMaterial_Click(object sender, EventArgs e)
        {
            if (dgvMaterials.CurrentRow?.Tag is ProductionMaterial material)
            {
                _materials.Remove(material);
                RefreshMaterialGrid();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var name = txtName.Text.Trim();
            var output = outputProductSelect.SelectedProduct;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Recipe name is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (output == null)
            {
                MessageBox.Show("Select an output product.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_materials.Count == 0)
            {
                MessageBox.Show("Add at least one material.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _recipe.Name = name;
            _recipe.OutputProductId = output.Id;
            _recipe.OutputProductSku = output.Sku;
            _recipe.OutputProductName = output.Name;
            _recipe.Materials = _materials
                .Select(m => new ProductionMaterial
                {
                    ProductId = m.ProductId,
                    ProductSku = m.ProductSku,
                    ProductName = m.ProductName,
                    QuantityPerUnit = m.QuantityPerUnit
                })
                .ToList();

            if (_isNew)
                _repository.AddRecipe(_recipe);
            else
                _repository.UpdateRecipe(_recipe);

            DialogResult = DialogResult.OK;
            Close();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (outputProductSelect.IsInputFocused || outputProductSelect.IsDropDownOpen)
                {
                    outputProductSelect.TrySelectHighlightedProduct();
                    return true;
                }

                if (materialProductSelect.IsInputFocused || materialProductSelect.IsDropDownOpen)
                {
                    materialProductSelect.TrySelectHighlightedProduct();
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
