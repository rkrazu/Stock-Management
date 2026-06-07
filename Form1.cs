using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class Form1 : Form
    {
        private readonly StockRepository _repository = new StockRepository();

        public Form1()
        {
            InitializeComponent();
            ConfigureProductGrid();
            ConfigureTransactionGrid();
            WireEvents();
            _repository.Load();
            RefreshAll();
        }

        private void WireEvents()
        {
            btnSearch.Click += (s, e) => RefreshProducts();
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnStockIn.Click += (s, e) => OpenStockAdjust(TransactionType.StockIn);
            btnStockOut.Click += (s, e) => OpenStockAdjust(TransactionType.StockOut);
            menuAddProduct.Click += BtnAdd_Click;
            menuEditProduct.Click += BtnEdit_Click;
            menuDeleteProduct.Click += BtnDelete_Click;
            menuStockIn.Click += (s, e) => OpenStockAdjust(TransactionType.StockIn);
            menuStockOut.Click += (s, e) => OpenStockAdjust(TransactionType.StockOut);
            chkLowStockOnly.CheckedChanged += (s, e) => RefreshProducts();
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RefreshProducts();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
            dgvProducts.SelectionChanged += (s, e) => UpdateActionButtons();
            dgvProducts.CellDoubleClick += (s, e) => BtnEdit_Click(s, e);
        }

        private void ConfigureProductGrid()
        {
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.Columns.Clear();
            dgvProducts.Columns.Add("Sku", "SKU");
            dgvProducts.Columns.Add("Name", "Name");
            dgvProducts.Columns.Add("Category", "Category");
            dgvProducts.Columns.Add("Quantity", "Qty");
            dgvProducts.Columns.Add("ReorderLevel", "Reorder");
            dgvProducts.Columns.Add("UnitPrice", "Unit Price");
            dgvProducts.Columns.Add("StockValue", "Value");
            dgvProducts.Columns.Add("Status", "Status");

            dgvProducts.Columns["UnitPrice"].DefaultCellStyle.Format = "C2";
            dgvProducts.Columns["StockValue"].DefaultCellStyle.Format = "C2";
            dgvProducts.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProducts.Columns["ReorderLevel"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void ConfigureTransactionGrid()
        {
            dgvTransactions.AutoGenerateColumns = false;
            dgvTransactions.Columns.Clear();
            dgvTransactions.Columns.Add("Timestamp", "Date/Time");
            dgvTransactions.Columns.Add("Type", "Type");
            dgvTransactions.Columns.Add("ProductSku", "SKU");
            dgvTransactions.Columns.Add("ProductName", "Product");
            dgvTransactions.Columns.Add("Quantity", "Qty");
            dgvTransactions.Columns.Add("Notes", "Notes");

            dgvTransactions.Columns["Timestamp"].DefaultCellStyle.Format = "g";
            dgvTransactions.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void RefreshAll()
        {
            RefreshHeader();
            RefreshProducts();
            RefreshTransactions();
            UpdateActionButtons();
        }

        private void RefreshHeader()
        {
            var count = _repository.Data.Products.Count;
            var low = _repository.LowStockCount;
            var value = _repository.TotalInventoryValue;
            lblStats.Text =
                $"{count} product(s)  |  {low} low stock  |  Total value: {value:C2}";
        }

        private void RefreshProducts()
        {
            var selectedId = GetSelectedProduct()?.Id;
            dgvProducts.Rows.Clear();

            var products = _repository.SearchProducts(txtSearch.Text);
            if (chkLowStockOnly.Checked)
                products = products.Where(p => p.IsLowStock);

            foreach (var p in products)
            {
                var idx = dgvProducts.Rows.Add(
                    p.Sku,
                    p.Name,
                    p.Category,
                    p.Quantity,
                    p.ReorderLevel,
                    p.UnitPrice,
                    p.StockValue,
                    p.IsLowStock ? "LOW" : "OK");

                dgvProducts.Rows[idx].Tag = p;

                if (p.IsLowStock)
                {
                    dgvProducts.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226);
                    dgvProducts.Rows[idx].DefaultCellStyle.ForeColor = Color.FromArgb(153, 27, 27);
                }
            }

            if (selectedId.HasValue)
            {
                foreach (DataGridViewRow row in dgvProducts.Rows)
                {
                    if (row.Tag is Product prod && prod.Id == selectedId.Value)
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }

            statusLabel.Text = $"Showing {dgvProducts.Rows.Count} item(s)";
            RefreshHeader();
        }

        private void RefreshTransactions()
        {
            dgvTransactions.Rows.Clear();
            foreach (var t in _repository.Data.Transactions.OrderByDescending(x => x.Timestamp))
            {
                var typeLabel = t.Type == TransactionType.StockIn ? "IN" : "OUT";
                dgvTransactions.Rows.Add(t.Timestamp, typeLabel, t.ProductSku, t.ProductName, t.Quantity, t.Notes);
            }
        }

        private Product GetSelectedProduct() =>
            dgvProducts.CurrentRow?.Tag as Product;

        private void UpdateActionButtons()
        {
            var hasSelection = GetSelectedProduct() != null;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
            btnStockIn.Enabled = hasSelection;
            btnStockOut.Enabled = hasSelection;
            menuEditProduct.Enabled = hasSelection;
            menuDeleteProduct.Enabled = hasSelection;
            menuStockIn.Enabled = hasSelection;
            menuStockOut.Enabled = hasSelection;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new ProductEditForm(_repository))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    RefreshAll();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var product = GetSelectedProduct();
            if (product == null)
            {
                MessageBox.Show("Select a product first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var latest = _repository.GetProduct(product.Id) ?? product;
            using (var form = new ProductEditForm(_repository, latest))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    RefreshAll();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var product = GetSelectedProduct();
            if (product == null) return;

            var confirm = MessageBox.Show(
                $"Delete \"{product.Name}\" and its transaction history?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            _repository.DeleteProduct(product.Id);
            RefreshAll();
        }

        private void OpenStockAdjust(TransactionType type)
        {
            var product = GetSelectedProduct();
            if (product == null)
            {
                MessageBox.Show("Select a product first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var latest = _repository.GetProduct(product.Id) ?? product;
            using (var form = new StockAdjustForm(_repository, latest, type))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    RefreshAll();
            }
        }
    }
}
