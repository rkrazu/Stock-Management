using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Data;
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
            ConfigureInvoiceGrid();
            ConfigureProductionGrids();
            ConfigureTransactionGrid();
            ConfigureTransactionFilters();
            WireEvents();
            UiStyles.Apply(this);
            lblPasswordRules.Text = PasswordPolicy.RequirementsText;
            lstSettingsNav.SelectedIndex = 0;
            ShowSettingsSection(0);
            _repository.Load();
            _allowGridSelection = false;
            ConfigureFooter();
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

            if (tabMain.SelectedTab == tabSettings)
            {
                RefreshSettingsTab();
                if (lstSettingsNav.SelectedIndex < 0)
                    lstSettingsNav.SelectedIndex = 0;
            }

            ResetGridSelections();
        }

        private void LstSettingsNav_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSettingsNav.SelectedIndex < 0)
                return;

            ShowSettingsSection(lstSettingsNav.SelectedIndex);
        }

        private void ShowSettingsSection(int index)
        {
            panelSettingsBackup.Visible = index == 0;
            panelSettingsPassword.Visible = index == 1;

            if (index == 1)
                ClearPasswordChangeFields();
        }

        private void ClearPasswordChangeFields()
        {
            txtCurrentPassword.Clear();
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();
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
            ClearGridSelection(dgvInvoices);
            ClearGridSelection(dgvRecipes);
            ClearGridSelection(dgvProductionOrders);
            ClearGridSelection(dgvTransactions);
            UpdateActionButtons();
            UpdateCustomerButtons();
            UpdateInvoiceButtons();
            UpdateProductionButtons();
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
            btnInvoiceSearch.Click += (s, e) => RefreshInvoices(preserveSelection: false);
            btnInvoiceReset.Click += (s, e) => ResetInvoiceFilters();
            btnViewInvoice.Click += BtnViewInvoice_Click;
            btnPrintInvoice.Click += BtnPrintInvoice_Click;
            dgvInvoices.SelectionChanged += (s, e) =>
            {
                GuardGridSelection(dgvInvoices);
                UpdateInvoiceButtons();
            };
            dgvInvoices.CellDoubleClick += (s, e) => BtnViewInvoice_Click(s, e);
            txtInvoiceSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RefreshInvoices(preserveSelection: false);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
            btnProductionSearch.Click += (s, e) => RefreshProduction(preserveSelection: false);
            btnProductionReset.Click += (s, e) => ResetProductionFilters();
            btnAddRecipe.Click += BtnAddRecipe_Click;
            btnEditRecipe.Click += BtnEditRecipe_Click;
            btnDeleteRecipe.Click += BtnDeleteRecipe_Click;
            btnRunProduction.Click += BtnRunProduction_Click;
            dgvRecipes.SelectionChanged += (s, e) =>
            {
                GuardGridSelection(dgvRecipes);
                UpdateProductionButtons();
            };
            dgvRecipes.CellDoubleClick += (s, e) => BtnEditRecipe_Click(s, e);
            txtProductionSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RefreshProduction(preserveSelection: false);
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
            dgvInvoices.VisibleChanged += Grid_VisibleChanged;
            dgvRecipes.VisibleChanged += Grid_VisibleChanged;
            dgvProductionOrders.VisibleChanged += Grid_VisibleChanged;
            dgvTransactions.VisibleChanged += Grid_VisibleChanged;
            btnBackupDatabase.Click += BtnBackupDatabase_Click;
            btnRestoreDatabase.Click += BtnRestoreDatabase_Click;
            lstSettingsNav.SelectedIndexChanged += LstSettingsNav_SelectedIndexChanged;
            btnChangePassword.Click += BtnChangePassword_Click;
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

        private void ConfigureInvoiceGrid()
        {
            dgvInvoices.AutoGenerateColumns = false;
            dgvInvoices.Columns.Clear();
            dgvInvoices.Columns.Add("CreatedAt", "Date/Time");
            dgvInvoices.Columns.Add("InvoiceNumber", "Invoice #");
            dgvInvoices.Columns.Add("CustomerName", "Customer");
            dgvInvoices.Columns.Add("ProductSummary", "Product");
            dgvInvoices.Columns.Add("TotalAmount", "Total");
            dgvInvoices.Columns.Add("Notes", "Notes");

            dgvInvoices.Columns["CreatedAt"].DefaultCellStyle.Format = "g";
            dgvInvoices.Columns["TotalAmount"].DefaultCellStyle.Format = "C2";
            dgvInvoices.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void ConfigureProductionGrids()
        {
            dgvRecipes.AutoGenerateColumns = false;
            dgvRecipes.Columns.Clear();
            dgvRecipes.Columns.Add("Name", "Recipe");
            dgvRecipes.Columns.Add("OutputSku", "Output SKU");
            dgvRecipes.Columns.Add("OutputName", "Output Product");
            dgvRecipes.Columns.Add("MaterialCount", "Materials");

            dgvProductionOrders.AutoGenerateColumns = false;
            dgvProductionOrders.Columns.Clear();
            dgvProductionOrders.Columns.Add("Timestamp", "Date/Time");
            dgvProductionOrders.Columns.Add("ProductionNumber", "Production #");
            dgvProductionOrders.Columns.Add("RecipeName", "Recipe");
            dgvProductionOrders.Columns.Add("OutputSku", "Output SKU");
            dgvProductionOrders.Columns.Add("OutputName", "Output Product");
            dgvProductionOrders.Columns.Add("QuantityProduced", "Qty");
            dgvProductionOrders.Columns.Add("TotalOutputValue", "Value");
            dgvProductionOrders.Columns.Add("Notes", "Notes");

            dgvProductionOrders.Columns["Timestamp"].DefaultCellStyle.Format = "g";
            dgvProductionOrders.Columns["QuantityProduced"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProductionOrders.Columns["TotalOutputValue"].DefaultCellStyle.Format = "C2";
            dgvProductionOrders.Columns["TotalOutputValue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
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

        private void ResetProductionFilters()
        {
            txtProductionSearch.Clear();
            RefreshProduction(preserveSelection: false);
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

        private void ResetInvoiceFilters()
        {
            txtInvoiceSearch.Clear();
            RefreshInvoices(preserveSelection: false);
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
            RefreshInvoices();
            RefreshProduction();
            RefreshTransactions();
            UpdateActionButtons();
            UpdateCustomerButtons();
            UpdateInvoiceButtons();
            UpdateProductionButtons();
        }

        private void RefreshHeader()
        {
            var count = _repository.ProductCount;
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

        private void RefreshInvoices(bool preserveSelection = true)
        {
            var selectedId = preserveSelection ? GetSelectedInvoice()?.Id : null;
            dgvInvoices.Rows.Clear();

            foreach (var invoice in _repository.SearchInvoices(txtInvoiceSearch.Text))
            {
                var idx = dgvInvoices.Rows.Add(
                    invoice.CreatedAt,
                    invoice.InvoiceNumber,
                    invoice.CustomerName ?? string.Empty,
                    FormatInvoiceProductSummary(invoice),
                    invoice.TotalAmount,
                    invoice.Notes ?? string.Empty);
                dgvInvoices.Rows[idx].Tag = invoice;
            }

            if (selectedId.HasValue)
            {
                var reselected = false;
                foreach (DataGridViewRow row in dgvInvoices.Rows)
                {
                    if (row.Tag is Invoice inv && inv.Id == selectedId.Value)
                    {
                        _allowGridSelection = true;
                        row.Selected = true;
                        reselected = true;
                        break;
                    }
                }

                if (!reselected)
                    ApplyNoSelection(dgvInvoices);
            }
            else
            {
                ApplyNoSelection(dgvInvoices);
            }

            UpdateInvoiceButtons();
        }

        private static string FormatInvoiceProductSummary(Invoice invoice)
        {
            if (invoice.Items == null || invoice.Items.Count == 0)
                return string.Empty;

            if (invoice.Items.Count == 1)
                return invoice.Items[0].ProductName ?? invoice.Items[0].ProductSku ?? string.Empty;

            return $"{invoice.Items.Count} items";
        }

        private void RefreshProduction(bool preserveSelection = true)
        {
            var selectedId = preserveSelection ? GetSelectedRecipe()?.Id : null;
            var term = txtProductionSearch.Text;

            dgvRecipes.Rows.Clear();
            foreach (var recipe in _repository.SearchRecipes(term))
            {
                var materialCount = recipe.Materials?.Count ?? 0;
                var idx = dgvRecipes.Rows.Add(recipe.Name, recipe.OutputProductSku, recipe.OutputProductName, materialCount);
                dgvRecipes.Rows[idx].Tag = recipe;
            }

            if (selectedId.HasValue)
            {
                var reselected = false;
                foreach (DataGridViewRow row in dgvRecipes.Rows)
                {
                    if (row.Tag is ProductionRecipe recipe && recipe.Id == selectedId.Value)
                    {
                        _allowGridSelection = true;
                        row.Selected = true;
                        reselected = true;
                        break;
                    }
                }

                if (!reselected)
                    ApplyNoSelection(dgvRecipes);
            }
            else
            {
                ApplyNoSelection(dgvRecipes);
            }

            dgvProductionOrders.Rows.Clear();
            foreach (var order in _repository.SearchProductionOrders(term))
            {
                dgvProductionOrders.Rows.Add(
                    order.Timestamp,
                    order.ProductionNumber,
                    order.RecipeName,
                    order.OutputProductSku,
                    order.OutputProductName,
                    order.QuantityProduced,
                    order.TotalOutputValue,
                    order.Notes);
            }

            ApplyNoSelection(dgvProductionOrders);
            UpdateProductionButtons();
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
                    t.IsSale ? (t.InvoiceNumber ?? string.Empty) : string.Empty,
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

        private Invoice GetSelectedInvoice()
        {
            if (dgvInvoices.SelectedRows.Count == 0) return null;
            return dgvInvoices.SelectedRows[0].Tag as Invoice;
        }

        private ProductionRecipe GetSelectedRecipe()
        {
            if (dgvRecipes.SelectedRows.Count == 0) return null;
            return dgvRecipes.SelectedRows[0].Tag as ProductionRecipe;
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

        private void UpdateInvoiceButtons()
        {
            var hasSelection = GetSelectedInvoice() != null;
            btnViewInvoice.Enabled = hasSelection;
            btnPrintInvoice.Enabled = hasSelection;
        }

        private void UpdateProductionButtons()
        {
            var hasSelection = GetSelectedRecipe() != null;
            btnEditRecipe.Enabled = hasSelection;
            btnDeleteRecipe.Enabled = hasSelection;
            btnRunProduction.Enabled = hasSelection;
        }

        private void BtnAddRecipe_Click(object sender, EventArgs e)
        {
            using (var form = new RecipeEditForm(_repository))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    RefreshAll();
            }
        }

        private void BtnEditRecipe_Click(object sender, EventArgs e)
        {
            var recipe = GetSelectedRecipe();
            if (recipe == null)
            {
                MessageBox.Show("Select a recipe first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var latest = _repository.GetRecipe(recipe.Id) ?? recipe;
            using (var form = new RecipeEditForm(_repository, latest))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    RefreshAll();
            }
        }

        private void BtnDeleteRecipe_Click(object sender, EventArgs e)
        {
            var recipe = GetSelectedRecipe();
            if (recipe == null) return;

            var confirm = MessageBox.Show(
                $"Delete recipe \"{recipe.Name}\"?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            _repository.DeleteRecipe(recipe.Id);
            RefreshAll();
        }

        private void BtnRunProduction_Click(object sender, EventArgs e)
        {
            var recipe = GetSelectedRecipe();
            if (recipe == null)
            {
                MessageBox.Show("Select a recipe first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var latest = _repository.GetRecipe(recipe.Id) ?? recipe;
            using (var form = new ProductionRunForm(_repository, latest))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    RefreshAll();
            }
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

        private void BtnViewInvoice_Click(object sender, EventArgs e)
        {
            var invoice = GetSelectedInvoice();
            if (invoice == null)
            {
                MessageBox.Show("Select an invoice first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var latest = _repository.GetInvoice(invoice.Id) ?? invoice;
            using (var form = new InvoiceForm(_repository, latest))
                form.ShowDialog(this);
        }

        private void BtnPrintInvoice_Click(object sender, EventArgs e)
        {
            var invoice = GetSelectedInvoice();
            if (invoice == null)
            {
                MessageBox.Show("Select an invoice first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var latest = _repository.GetInvoice(invoice.Id) ?? invoice;
            InvoiceDocumentBuilder.Print(latest, isDraft: false);
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

        private void ConfigureFooter()
        {
            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "rk-razu.png");
            if (!File.Exists(imagePath))
                return;

            try
            {
                const int avatarSize = 42;
                using (var source = Image.FromFile(imagePath))
                    statusFooter.Image = CreateFooterAvatar(source, avatarSize);

                statusStrip.ImageScalingSize = new Size(avatarSize, avatarSize);
            }
            catch
            {
                // Keep copyright text if the image cannot be loaded.
            }
        }

        private static Image CreateFooterAvatar(Image source, int size)
        {
            var avatar = new Bitmap(size, size);
            using (var graphics = Graphics.FromImage(avatar))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                var cropSize = Math.Min(source.Width, source.Height);
                var cropX = (source.Width - cropSize) / 2;
                var cropY = (source.Height - cropSize) / 2;
                var sourceRect = new Rectangle(cropX, cropY, cropSize, cropSize);
                var destRect = new Rectangle(0, 0, size, size);

                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(1, 1, size - 2, size - 2);
                    graphics.SetClip(path);
                    graphics.DrawImage(source, destRect, sourceRect, GraphicsUnit.Pixel);
                }
            }

            return avatar;
        }

        private void RefreshSettingsTab()
        {
            try
            {
                lblSettingsServerValue.Text = DatabaseBackupService.ServerName;
                lblSettingsDatabaseValue.Text = DatabaseBackupService.DatabaseName;
            }
            catch (Exception ex)
            {
                lblSettingsServerValue.Text = "Unavailable";
                lblSettingsDatabaseValue.Text = ex.Message;
            }
        }

        private void BtnBackupDatabase_Click(object sender, EventArgs e)
        {
            var defaultFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "StockManagement Backups");

            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "Save Database Backup";
                dialog.Filter = "SQL Server Backup (*.bak)|*.bak";
                dialog.DefaultExt = "bak";
                dialog.FileName = $"{DatabaseBackupService.DatabaseName}_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                dialog.InitialDirectory = Directory.Exists(defaultFolder)
                    ? defaultFolder
                    : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    UseWaitCursor = true;
                    btnBackupDatabase.Enabled = false;
                    btnRestoreDatabase.Enabled = false;

                    _repository.Save();
                    DatabaseBackupService.Backup(dialog.FileName);

                    MessageBox.Show(
                        "Backup completed successfully.\r\n\r\n" + dialog.FileName,
                        Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Backup failed.\r\n\r\n" + ex.Message,
                        Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    UseWaitCursor = false;
                    btnBackupDatabase.Enabled = true;
                    btnRestoreDatabase.Enabled = true;
                }
            }
        }

        private void BtnRestoreDatabase_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Restore will replace ALL current database data with the selected backup.\r\n\r\nContinue?",
                "Confirm Restore",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select Database Backup";
                dialog.Filter = "SQL Server Backup (*.bak)|*.bak";
                dialog.CheckFileExists = true;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    UseWaitCursor = true;
                    btnBackupDatabase.Enabled = false;
                    btnRestoreDatabase.Enabled = false;
                    tabMain.Enabled = false;

                    _repository.Save();
                    DatabaseBackupService.Restore(dialog.FileName);
                    AuthenticationService.EnsureDefaultPassword();
                    _repository.Load();
                    RefreshAll();
                    RefreshSettingsTab();

                    MessageBox.Show(
                        "Database restored successfully.",
                        Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Restore failed.\r\n\r\n" + ex.Message,
                        Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    UseWaitCursor = false;
                    btnBackupDatabase.Enabled = true;
                    btnRestoreDatabase.Enabled = true;
                    tabMain.Enabled = true;
                }
            }
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            var error = AuthenticationService.ChangePassword(
                txtCurrentPassword.Text,
                txtNewPassword.Text,
                txtConfirmPassword.Text);

            if (error != null)
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ClearPasswordChangeFields();
            MessageBox.Show(
                "Password changed successfully.",
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
