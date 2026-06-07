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
        private bool _isFormLoaded;
        private bool _allowGridSelection;
        private bool _filterInActive;
        private bool _filterOutActive;

        public Form1()
        {
            InitializeComponent();
            ConfigureProductGrid();
            ConfigureCustomerGrid();
            ConfigureTransactionGrid();
            ConfigureTransactionFilters();
            WireEvents();
            _repository.Load();
            _allowGridSelection = false;
            RefreshAll();
            Shown += Form1_Shown;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            _isFormLoaded = true;
            ResetGridSelections();
        }

        private void TabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isFormLoaded) return;
            ResetGridSelections();
        }

        private void ResetGridSelections()
        {
            _allowGridSelection = false;
            BeginInvokeWhenReady(() =>
            {
                ClearAllGridSelections();
                _allowGridSelection = true;
            });
        }

        private void BeginInvokeWhenReady(Action action)
        {
            if (!IsHandleCreated)
                return;

            BeginInvoke(action);
        }

        private void ClearAllGridSelections()
        {
            ClearGridSelection(dgvProducts);
            ClearGridSelection(dgvCustomers);
            ClearGridSelection(dgvTransactions);
            UpdateActionButtons();
            UpdateCustomerButtons();
        }

        private void GuardGridSelection(DataGridView grid)
        {
            if (_allowGridSelection || grid.SelectedRows.Count == 0) return;
            ClearGridSelection(grid);
        }

        private void ApplyNoSelection(DataGridView grid)
        {
            _allowGridSelection = false;
            ClearGridSelection(grid);
            BeginInvokeWhenReady(() => _allowGridSelection = true);
        }

        private static void ClearGridSelection(DataGridView grid)
        {
            if (grid.RowCount == 0) return;

            grid.ClearSelection();
            foreach (DataGridViewRow row in grid.Rows)
                row.Selected = false;

            try
            {
                grid.CurrentCell = null;
            }
            catch (InvalidOperationException)
            {
                // Grid not ready yet; row deselection above is enough.
            }
        }

        private void WireEvents()
        {
            btnSearch.Click += (s, e) => RefreshProducts(preserveSelection: false);
            btnReset.Click += (s, e) => ResetProductFilters();
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnStockIn.Click += (s, e) => OpenStockAdjust(TransactionType.StockIn);
            btnStockOut.Click += (s, e) => OpenStockAdjust(TransactionType.StockOut);
            chkLowStockOnly.CheckedChanged += (s, e) => RefreshProducts(preserveSelection: false);
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RefreshProducts(preserveSelection: false);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
            dgvProducts.SelectionChanged += (s, e) =>
            {
                GuardGridSelection(dgvProducts);
                UpdateActionButtons();
            };
            dgvProducts.CellDoubleClick += (s, e) => BtnEdit_Click(s, e);
            btnCustomerSearch.Click += (s, e) => RefreshCustomers(preserveSelection: false);
            btnCustomerReset.Click += (s, e) => ResetCustomerFilters();
            btnAddCustomer.Click += BtnAddCustomer_Click;
            btnEditCustomer.Click += BtnEditCustomer_Click;
            btnDeleteCustomer.Click += BtnDeleteCustomer_Click;
            dgvCustomers.SelectionChanged += (s, e) =>
            {
                GuardGridSelection(dgvCustomers);
                UpdateCustomerButtons();
            };
            dgvCustomers.CellDoubleClick += (s, e) => BtnEditCustomer_Click(s, e);
            txtCustomerSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RefreshCustomers(preserveSelection: false);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
            tabMain.SelectedIndexChanged += TabMain_SelectedIndexChanged;
            btnTxnSearch.Click += (s, e) => RefreshTransactions();
            btnTxnReset.Click += (s, e) => ResetTransactionFilters();
            btnFilterIn.Click += BtnFilterIn_Click;
            btnFilterOut.Click += BtnFilterOut_Click;
            chkTxnDateRange.CheckedChanged += ChkTxnDateRange_CheckedChanged;
            dtpTxnFrom.ValueChanged += (s, e) => { if (chkTxnDateRange.Checked) RefreshTransactions(); };
            dtpTxnTo.ValueChanged += (s, e) => { if (chkTxnDateRange.Checked) RefreshTransactions(); };
            txtTxnSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RefreshTransactions();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
            dgvTransactions.SelectionChanged += (s, e) => GuardGridSelection(dgvTransactions);
            dgvProducts.VisibleChanged += Grid_VisibleChanged;
            dgvCustomers.VisibleChanged += Grid_VisibleChanged;
            dgvTransactions.VisibleChanged += Grid_VisibleChanged;
        }

        private void Grid_VisibleChanged(object sender, EventArgs e)
        {
            var grid = (DataGridView)sender;
            if (!grid.Visible || !_isFormLoaded) return;

            BeginInvokeWhenReady(() =>
            {
                if (!grid.Visible) return;
                _allowGridSelection = false;
                ClearGridSelection(grid);
                _allowGridSelection = true;
            });
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

        private void ConfigureCustomerGrid()
        {
            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.Columns.Clear();
            dgvCustomers.Columns.Add("Name", "Name");
            dgvCustomers.Columns.Add("Phone", "Phone");
            dgvCustomers.Columns.Add("Email", "Email");
            dgvCustomers.Columns.Add("Address", "Address");
        }

        private void ConfigureTransactionGrid()
        {
            dgvTransactions.AutoGenerateColumns = false;
            dgvTransactions.Columns.Clear();
            dgvTransactions.Columns.Add("Timestamp", "Date/Time");
            dgvTransactions.Columns.Add("InvoiceNumber", "Invoice");
            dgvTransactions.Columns.Add("Type", "Type");
            dgvTransactions.Columns.Add("ProductSku", "SKU");
            dgvTransactions.Columns.Add("ProductName", "Product");
            dgvTransactions.Columns.Add("Quantity", "Qty");
            dgvTransactions.Columns.Add("TotalValue", "Value");
            dgvTransactions.Columns.Add("CustomerName", "Customer");
            dgvTransactions.Columns.Add("Notes", "Notes");

            dgvTransactions.Columns["Timestamp"].DefaultCellStyle.Format = "g";
            dgvTransactions.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvTransactions.Columns["TotalValue"].DefaultCellStyle.Format = "C2";
            dgvTransactions.Columns["TotalValue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void ConfigureTransactionFilters()
        {
            ResetTransactionDateDefaults();
            UpdateFilterButtonStyles();
        }

        private void ResetProductFilters()
        {
            txtSearch.Clear();
            chkLowStockOnly.Checked = false;
            RefreshProducts(preserveSelection: false);
        }

        private void ResetCustomerFilters()
        {
            txtCustomerSearch.Clear();
            RefreshCustomers(preserveSelection: false);
        }

        private void ResetTransactionFilters()
        {
            txtTxnSearch.Clear();
            _filterInActive = false;
            _filterOutActive = false;
            UpdateFilterButtonStyles();
            chkTxnDateRange.Checked = false;
            ResetTransactionDateDefaults();
            RefreshTransactions();
        }

        private void ResetTransactionDateDefaults()
        {
            dtpTxnTo.Value = DateTime.Today;
            dtpTxnFrom.Value = DateTime.Today.AddMonths(-1);
            dtpTxnFrom.Enabled = false;
            dtpTxnTo.Enabled = false;
        }

        private void BtnFilterIn_Click(object sender, EventArgs e)
        {
            _filterInActive = !_filterInActive;
            if (_filterInActive)
                _filterOutActive = false;
            UpdateFilterButtonStyles();
            RefreshTransactions();
        }

        private void BtnFilterOut_Click(object sender, EventArgs e)
        {
            _filterOutActive = !_filterOutActive;
            if (_filterOutActive)
                _filterInActive = false;
            UpdateFilterButtonStyles();
            RefreshTransactions();
        }

        private void ChkTxnDateRange_CheckedChanged(object sender, EventArgs e)
        {
            dtpTxnFrom.Enabled = chkTxnDateRange.Checked;
            dtpTxnTo.Enabled = chkTxnDateRange.Checked;
            RefreshTransactions();
        }

        private void UpdateFilterButtonStyles()
        {
            var activeColor = Color.FromArgb(37, 99, 235);
            var activeFore = Color.White;

            btnFilterIn.BackColor = _filterInActive ? activeColor : SystemColors.Control;
            btnFilterIn.ForeColor = _filterInActive ? activeFore : SystemColors.ControlText;
            btnFilterOut.BackColor = _filterOutActive ? activeColor : SystemColors.Control;
            btnFilterOut.ForeColor = _filterOutActive ? activeFore : SystemColors.ControlText;
        }

        private bool TryGetTransactionDateRange(out DateTime? fromDate, out DateTime? toDate)
        {
            fromDate = null;
            toDate = null;

            if (!chkTxnDateRange.Checked)
                return true;

            if (dtpTxnFrom.Value.Date > dtpTxnTo.Value.Date)
            {
                MessageBox.Show("'From' date cannot be after 'To' date.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            fromDate = dtpTxnFrom.Value.Date;
            toDate = dtpTxnTo.Value.Date;
            return true;
        }

        private void RefreshAll()
        {
            RefreshHeader();
            RefreshProducts();
            RefreshCustomers();
            RefreshTransactions();
            UpdateActionButtons();
            UpdateCustomerButtons();
        }

        private void RefreshHeader()
        {
            var count = _repository.Data.Products.Count;
            var low = _repository.LowStockCount;
            var value = _repository.TotalInventoryValue;
            lblStats.Text =
                $"{count} product(s)  |  {low} low stock  |  Total value: {value:C2}";
        }

        private void RefreshProducts(bool preserveSelection = true)
        {
            var selectedId = preserveSelection ? GetSelectedProduct()?.Id : null;
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
                var reselected = false;
                foreach (DataGridViewRow row in dgvProducts.Rows)
                {
                    if (row.Tag is Product prod && prod.Id == selectedId.Value)
                    {
                        _allowGridSelection = true;
                        row.Selected = true;
                        reselected = true;
                        break;
                    }
                }

                if (!reselected)
                    ApplyNoSelection(dgvProducts);
            }
            else
            {
                ApplyNoSelection(dgvProducts);
            }

            statusLabel.Text = $"Showing {dgvProducts.Rows.Count} item(s)";
            RefreshHeader();
            UpdateActionButtons();
        }

        private void RefreshCustomers(bool preserveSelection = true)
        {
            var selectedId = preserveSelection ? GetSelectedCustomer()?.Id : null;
            dgvCustomers.Rows.Clear();

            foreach (var c in _repository.SearchCustomers(txtCustomerSearch.Text))
            {
                var idx = dgvCustomers.Rows.Add(c.Name, c.Phone, c.Email, c.Address);
                dgvCustomers.Rows[idx].Tag = c;
            }

            if (selectedId.HasValue)
            {
                var reselected = false;
                foreach (DataGridViewRow row in dgvCustomers.Rows)
                {
                    if (row.Tag is Customer cust && cust.Id == selectedId.Value)
                    {
                        _allowGridSelection = true;
                        row.Selected = true;
                        reselected = true;
                        break;
                    }
                }

                if (!reselected)
                    ApplyNoSelection(dgvCustomers);
            }
            else
            {
                ApplyNoSelection(dgvCustomers);
            }

            UpdateCustomerButtons();
        }

        private void RefreshTransactions()
        {
            if (!TryGetTransactionDateRange(out var fromDate, out var toDate))
                return;

            dgvTransactions.Rows.Clear();

            foreach (var t in _repository.SearchTransactions(
                txtTxnSearch.Text,
                _filterInActive,
                _filterOutActive,
                fromDate,
                toDate))
            {
                var typeLabel = t.Type == TransactionType.StockIn ? "IN" : "OUT";
                dgvTransactions.Rows.Add(
                    t.Timestamp,
                    t.InvoiceNumber,
                    typeLabel,
                    t.ProductSku,
                    t.ProductName,
                    t.Quantity,
                    t.TotalValue,
                    t.CustomerName ?? string.Empty,
                    t.Notes);
            }

            ApplyNoSelection(dgvTransactions);
        }

        private Product GetSelectedProduct()
        {
            if (dgvProducts.SelectedRows.Count == 0) return null;
            return dgvProducts.SelectedRows[0].Tag as Product;
        }

        private Customer GetSelectedCustomer()
        {
            if (dgvCustomers.SelectedRows.Count == 0) return null;
            return dgvCustomers.SelectedRows[0].Tag as Customer;
        }

        private void UpdateActionButtons()
        {
            var hasSelection = GetSelectedProduct() != null;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
            btnStockIn.Enabled = hasSelection;
            btnStockOut.Enabled = hasSelection;
        }

        private void UpdateCustomerButtons()
        {
            var hasSelection = GetSelectedCustomer() != null;
            btnEditCustomer.Enabled = hasSelection;
            btnDeleteCustomer.Enabled = hasSelection;
        }

        private void BtnAddCustomer_Click(object sender, EventArgs e)
        {
            using (var form = new CustomerEditForm(_repository))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    RefreshAll();
            }
        }

        private void BtnEditCustomer_Click(object sender, EventArgs e)
        {
            var customer = GetSelectedCustomer();
            if (customer == null)
            {
                MessageBox.Show("Select a customer first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var latest = _repository.GetCustomer(customer.Id) ?? customer;
            using (var form = new CustomerEditForm(_repository, latest))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    RefreshAll();
            }
        }

        private void BtnDeleteCustomer_Click(object sender, EventArgs e)
        {
            var customer = GetSelectedCustomer();
            if (customer == null) return;

            var confirm = MessageBox.Show(
                $"Delete customer \"{customer.Name}\"?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            _repository.DeleteCustomer(customer.Id);
            RefreshAll();
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
