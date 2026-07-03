using System;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Controls;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class ProductionRunForm : Form
    {
        private readonly StockRepository _repository;
        private readonly ProductionRecipe _recipe;

        public ProductionRunForm(StockRepository repository, ProductionRecipe recipe)
        {
            _repository = repository;
            _recipe = recipe;
            InitializeComponent();
            UiStyles.Apply(this);

            lblRecipe.Text = $"{recipe.Name}  →  {recipe.OutputProductName} ({recipe.OutputProductSku})";
            ConfigureMaterialGrid();
            GridExportUi.Enable(dgvMaterials, "Production Materials");
            RefreshMaterialGrid();
        }

        private void ConfigureMaterialGrid()
        {
            dgvMaterials.AutoGenerateColumns = false;
            dgvMaterials.Columns.Clear();
            dgvMaterials.Columns.Add("Sku", "SKU");
            dgvMaterials.Columns.Add("Name", "Material");
            dgvMaterials.Columns.Add("Required", "Required Qty");
            dgvMaterials.Columns.Add("Available", "Available");
            dgvMaterials.Columns["Required"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvMaterials.Columns["Available"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void RefreshMaterialGrid()
        {
            var batchQty = (int)numBatchQty.Value;
            dgvMaterials.Rows.Clear();

            foreach (var material in _recipe.Materials)
            {
                var product = _repository.GetProduct(material.ProductId);
                var available = product?.Quantity ?? 0;
                var required = material.QuantityPerUnit * batchQty;
                var idx = dgvMaterials.Rows.Add(material.ProductSku, material.ProductName, required, available);
                if (available < required)
                    dgvMaterials.Rows[idx].DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(153, 27, 27);
            }
        }

        private void NumBatchQty_ValueChanged(object sender, EventArgs e)
        {
            RefreshMaterialGrid();
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            var batchQty = (int)numBatchQty.Value;
            var error = _repository.RunProduction(_recipe, batchQty, txtNotes.Text.Trim());

            if (error != null)
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshMaterialGrid();
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
