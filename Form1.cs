using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Stock_Managemnet.Data;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;
using Stock_Managemnet.Controls;

namespace Stock_Managemnet
{
    public partial class Form1 : Form
    {
        private readonly StockRepository _repository = new StockRepository();
        private bool _isFormLoaded;
        private bool _allowGridSelection;
        private bool _filterInActive;
        private bool _filterOutActive;
        private bool _suppressCategoryFilterChange;
        private readonly HashSet<Guid> _fgSelectedProductIds = new HashSet<Guid>();
        private bool _suppressFgCheckboxEvents;
        private bool? _selectAllHeaderState;

        public Form1()
        {
            InitializeComponent();
            InitializeAccountsUi();
            ConfigureProductGrid();
            ConfigureCustomerGrid();
            ConfigureSupplierGrid();
            ConfigureInvoiceGrid();
            ConfigureProductionGrids();
            ConfigureTransactionGrid();
            ConfigureTransactionFilters();
            ConfigureGridExports();
            WireEvents();
            WireAccountsEvents();
            UiStyles.Apply(this);
            lblPasswordRules.Text = PasswordPolicy.RequirementsText;
            lstSettingsNav.SelectedIndex = 0;
            ShowSettingsSection(0);
            _repository.Load();
            _allowGridSelection = false;
            ConfigureProductFilters();
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
            else if (tabMain.SelectedTab == tabAccounts)
            {
                RefreshAccountsTab();
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
            panelSettingsSoftware.Visible = index == 2;

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
            ClearGridSelection(dgvSuppliers);
            ClearGridSelection(dgvInvoices);
            ClearGridSelection(dgvRecipes);
            ClearGridSelection(dgvProductionOrders);
            ClearGridSelection(dgvTransactions);
            UpdateActionButtons();
            UpdateCustomerButtons();
            UpdateSupplierButtons();
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
            btnSearch.Click += (s, e) => RefreshProducts();
            btnReset.Click += (s, e) => ResetProductFilters();
            tabInventorySub.SelectedIndexChanged += TabInventorySub_SelectedIndexChanged;
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnStockIn.Click += (s, e) => OpenStockIn();
            btnStockOut.Click += (s, e) => OpenStockOut();
            chkLowStockOnly.CheckedChanged += (s, e) => RefreshProducts();
            cmbProductCategory.SelectedIndexChanged += CmbProductCategory_SelectedIndexChanged;
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RefreshProducts();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
            dgvProducts.SelectionChanged += (s, e) =>
            {
                GuardGridSelection(dgvProducts);
                UpdateActionButtons();
            };
            dgvProducts.CellValueChanged += DgvProducts_CellValueChanged;
            dgvProducts.CurrentCellDirtyStateChanged += DgvProducts_CurrentCellDirtyStateChanged;
            dgvProducts.CellPainting += DgvProducts_CellPainting;
            dgvProducts.ColumnHeaderMouseClick += DgvProducts_ColumnHeaderMouseClick;
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

            btnSupplierSearch.Click += (s, e) => RefreshSuppliers(preserveSelection: false);
            btnSupplierReset.Click += (s, e) => ResetSupplierFilters();
            btnAddSupplier.Click += BtnAddSupplier_Click;
            btnEditSupplier.Click += BtnEditSupplier_Click;
            btnDeleteSupplier.Click += BtnDeleteSupplier_Click;
            dgvSuppliers.SelectionChanged += (s, e) =>
            {
                GuardGridSelection(dgvSuppliers);
                UpdateSupplierButtons();
            };
            dgvSuppliers.CellDoubleClick += (s, e) => BtnEditSupplier_Click(s, e);
            txtSupplierSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RefreshSuppliers(preserveSelection: false);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
            btnInvoiceSearch.Click += (s, e) => RefreshInvoices(preserveSelection: false);
            btnInvoiceReset.Click += (s, e) => ResetInvoiceFilters();
            btnViewInvoice.Click += BtnViewInvoice_Click;
            btnPrintInvoice.Click += BtnPrintInvoice_Click;
            btnChalan.Click += BtnChalan_Click;
            btnVoidInvoice.Click += BtnVoidInvoice_Click;
            btnRestoreSale.Click += BtnRestoreSale_Click;
            btnCorrectionGuide.Click += BtnCorrectionGuide_Click;
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
            btnReverseProduction.Click += BtnReverseProduction_Click;
            btnRestoreProduction.Click += BtnRestoreProduction_Click;
            dgvProductionOrders.SelectionChanged += (s, e) =>
            {
                GuardGridSelection(dgvProductionOrders);
                UpdateProductionHistoryButtons();
            };
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
            dgvSuppliers.VisibleChanged += Grid_VisibleChanged;
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
            dgvProducts.ReadOnly = false;
            dgvProducts.Columns.Clear();

            var colSelect = new DataGridViewCheckBoxColumn
            {
                Name = "Select",
                HeaderText = "Select All",
                Width = 96,
                ReadOnly = false
            };
            dgvProducts.Columns.Add(colSelect);
            dgvProducts.Columns.Add("Sku", "SKU");
            dgvProducts.Columns.Add("Name", "Name");
            dgvProducts.Columns.Add("Category", "Category");
            dgvProducts.Columns.Add("Quantity", "Qty");
            dgvProducts.Columns.Add("ReorderLevel", "Reorder");
            dgvProducts.Columns.Add("UnitPrice", "Unit Price");
            dgvProducts.Columns.Add("StockValue", "Total Value");
            dgvProducts.Columns.Add("Status", "Status");

            foreach (DataGridViewColumn column in dgvProducts.Columns)
            {
                if (column.Name != "Select")
                    column.ReadOnly = true;
            }

            dgvProducts.Columns["UnitPrice"].DefaultCellStyle.Format = "C2";
            dgvProducts.Columns["StockValue"].DefaultCellStyle.Format = "C2";
            dgvProducts.Columns["StockValue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProducts.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProducts.Columns["ReorderLevel"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProducts.Columns["Select"].Visible = false;
        }

        private void DgvProducts_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (!dgvProducts.IsCurrentCellDirty)
                return;

            if (dgvProducts.CurrentCell is DataGridViewCheckBoxCell)
                dgvProducts.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DgvProducts_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_suppressFgCheckboxEvents || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvProducts.Columns[e.ColumnIndex].Name != "Select")
                return;

            var row = dgvProducts.Rows[e.RowIndex];
            if (!(row.Tag is Product product))
                return;

            var isChecked = row.Cells["Select"].Value is bool value && value;
            if (isChecked)
                _fgSelectedProductIds.Add(product.Id);
            else
                _fgSelectedProductIds.Remove(product.Id);

            UpdateSelectAllHeaderState();
            UpdateActionButtons();
        }

        private void DgvProducts_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex != -1 || e.ColumnIndex < 0)
                return;

            if (dgvProducts.Columns[e.ColumnIndex].Name != "Select")
                return;

            e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

            var checkSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, CheckBoxState.UncheckedNormal);
            var checkY = e.CellBounds.Top + (e.CellBounds.Height - checkSize.Height) / 2;
            var checkX = e.CellBounds.Left + 6;
            var checkRect = new Rectangle(checkX, checkY, checkSize.Width, checkSize.Height);

            CheckBoxState state;
            if (_selectAllHeaderState == null)
                state = CheckBoxState.MixedNormal;
            else if (_selectAllHeaderState.Value)
                state = CheckBoxState.CheckedNormal;
            else
                state = CheckBoxState.UncheckedNormal;

            CheckBoxRenderer.DrawCheckBox(e.Graphics, checkRect.Location, state);

            var textRect = new Rectangle(
                checkRect.Right + 6,
                e.CellBounds.Top,
                e.CellBounds.Width - checkRect.Width - 12,
                e.CellBounds.Height);
            TextRenderer.DrawText(
                e.Graphics,
                "Select All",
                dgvProducts.ColumnHeadersDefaultCellStyle.Font,
                textRect,
                dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            e.Handled = true;
        }

        private void DgvProducts_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
                return;

            if (dgvProducts.Columns[e.ColumnIndex].Name != "Select")
                return;

            ToggleSelectAllHeader();
        }

        private void ToggleSelectAllHeader()
        {
            ApplyVisibleProductCheckboxes(_selectAllHeaderState != true);
        }

        private void ApplyVisibleProductCheckboxes(bool selectVisible)
        {
            dgvProducts.EndEdit();

            try
            {
                dgvProducts.CurrentCell = null;
            }
            catch (InvalidOperationException)
            {
                // Grid may not allow clearing the current cell in some states.
            }

            _suppressFgCheckboxEvents = true;
            try
            {
                if (selectVisible)
                {
                    foreach (DataGridViewRow row in dgvProducts.Rows)
                    {
                        if (row.Tag is Product product)
                            _fgSelectedProductIds.Add(product.Id);

                        row.Cells["Select"].Value = true;
                    }
                }
                else
                {
                    _fgSelectedProductIds.Clear();
                    foreach (DataGridViewRow row in dgvProducts.Rows)
                        row.Cells["Select"].Value = false;
                }
            }
            finally
            {
                _suppressFgCheckboxEvents = false;
            }

            var selectColumnIndex = dgvProducts.Columns["Select"].Index;
            foreach (DataGridViewRow row in dgvProducts.Rows)
                dgvProducts.InvalidateCell(selectColumnIndex, row.Index);

            _selectAllHeaderState = selectVisible ? (bool?)true : false;
            dgvProducts.InvalidateColumn(selectColumnIndex);
            UpdateActionButtons();
        }

        private void UpdateSelectAllHeaderState()
        {
            if (dgvProducts.Rows.Count == 0)
            {
                _selectAllHeaderState = _fgSelectedProductIds.Count > 0 ? (bool?)null : false;
                dgvProducts.InvalidateColumn(dgvProducts.Columns["Select"].Index);
                return;
            }

            var visibleIds = dgvProducts.Rows
                .Cast<DataGridViewRow>()
                .Select(row => row.Tag as Product)
                .Where(product => product != null)
                .Select(product => product.Id)
                .ToList();

            if (visibleIds.Count == 0)
            {
                _selectAllHeaderState = _fgSelectedProductIds.Count > 0 ? (bool?)null : false;
            }
            else
            {
                var selectedVisibleCount = visibleIds.Count(id => _fgSelectedProductIds.Contains(id));
                if (selectedVisibleCount == 0)
                    _selectAllHeaderState = false;
                else if (selectedVisibleCount == visibleIds.Count)
                    _selectAllHeaderState = true;
                else
                    _selectAllHeaderState = null;
            }

            dgvProducts.InvalidateColumn(dgvProducts.Columns["Select"].Index);
        }

        private void ClearInventoryProductSelection()
        {
            ApplyVisibleProductCheckboxes(selectVisible: false);
        }

        private void ClearFgProductSelection()
        {
            ClearInventoryProductSelection();
        }

        private List<Product> GetFgCartProducts()
        {
            return _fgSelectedProductIds
                .Select(id => _repository.GetProduct(id))
                .Where(product => product != null && product.ProductType == ProductType.FG)
                .ToList();
        }

        private List<Product> GetRawMaterialCartProducts()
        {
            return _fgSelectedProductIds
                .Select(id => _repository.GetProduct(id))
                .Where(product => product != null && product.ProductType == ProductType.RawMaterial)
                .ToList();
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

        private void ConfigureSupplierGrid()
        {
            dgvSuppliers.AutoGenerateColumns = false;
            dgvSuppliers.Columns.Clear();
            dgvSuppliers.Columns.Add("Name", "Name");
            dgvSuppliers.Columns.Add("Phone", "Phone");
            dgvSuppliers.Columns.Add("Email", "Email");
            dgvSuppliers.Columns.Add("Address", "Address");
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
            dgvInvoices.Columns.Add("AmountPaid", "Paid");
            dgvInvoices.Columns.Add("BalanceDue", "Due");
            dgvInvoices.Columns.Add("Status", "Status");
            dgvInvoices.Columns.Add("Notes", "Notes");

            dgvInvoices.Columns["CreatedAt"].DefaultCellStyle.Format = "g";
            dgvInvoices.Columns["TotalAmount"].DefaultCellStyle.Format = "C2";
            dgvInvoices.Columns["AmountPaid"].DefaultCellStyle.Format = "C2";
            dgvInvoices.Columns["BalanceDue"].DefaultCellStyle.Format = "C2";
            dgvInvoices.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvInvoices.Columns["AmountPaid"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvInvoices.Columns["BalanceDue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void ConfigureProductionGrids()
        {
            dgvRecipes.AutoGenerateColumns = false;
            dgvRecipes.Columns.Clear();
            dgvRecipes.Columns.Add("Name", "Production Name");
            dgvRecipes.Columns.Add("OutputSku", "Output SKU");
            dgvRecipes.Columns.Add("OutputName", "Output Product");
            dgvRecipes.Columns.Add("MaterialCount", "Materials");

            dgvProductionOrders.AutoGenerateColumns = false;
            dgvProductionOrders.Columns.Clear();
            dgvProductionOrders.Columns.Add("Timestamp", "Date/Time");
            dgvProductionOrders.Columns.Add("ProductionNumber", "Production #");
            dgvProductionOrders.Columns.Add("RecipeName", "Production Name");
            dgvProductionOrders.Columns.Add("OutputSku", "Output SKU");
            dgvProductionOrders.Columns.Add("OutputName", "Output Product");
            dgvProductionOrders.Columns.Add("QuantityProduced", "Qty");
            dgvProductionOrders.Columns.Add("TotalOutputValue", "Value");
            dgvProductionOrders.Columns.Add("Status", "Status");
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

        private void ConfigureGridExports()
        {
            GridExportUi.Enable(dgvProducts, "Products");
            GridExportUi.Enable(dgvCustomers, "Customers");
            GridExportUi.Enable(dgvSuppliers, "Suppliers");
            GridExportUi.Enable(dgvInvoices, "Invoices");
            GridExportUi.Enable(dgvRecipes, "Productions");
            GridExportUi.Enable(dgvProductionOrders, "Production History");
            GridExportUi.Enable(dgvTransactions, "Transaction History");
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

        private void ConfigureProductFilters()
        {
            RefreshProductCategoryFilter();
            ApplyInventorySubTabSettings();
        }

        private void TabInventorySub_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyInventorySubTabSettings();
        }

        private bool IsFgInventoryTab => tabInventorySub.SelectedTab == tabInventoryFg;

        private ProductType GetActiveInventoryProductType() =>
            IsFgInventoryTab ? ProductType.FG : ProductType.RawMaterial;

        private void ApplyInventorySubTabSettings()
        {
            var isFg = IsFgInventoryTab;
            dgvProducts.MultiSelect = false;
            dgvProducts.Columns["Select"].Visible = true;
            btnStockIn.Visible = !isFg;
            btnStockOut.Visible = isFg;
            ClearInventoryProductSelection();
            RefreshProductCategoryFilter();
            RefreshProducts();
            UpdateActionButtons();
        }

        private void RefreshProductCategoryFilter()
        {
            var selected = cmbProductCategory.SelectedItem?.ToString();
            _suppressCategoryFilterChange = true;
            cmbProductCategory.Items.Clear();
            cmbProductCategory.Items.Add("All categories");
            foreach (var category in _repository.GetProductCategories(GetActiveInventoryProductType()))
                cmbProductCategory.Items.Add(category);

            if (!string.IsNullOrEmpty(selected) && cmbProductCategory.Items.Contains(selected))
                cmbProductCategory.SelectedItem = selected;
            else
                cmbProductCategory.SelectedIndex = 0;

            _suppressCategoryFilterChange = false;
        }

        private void CmbProductCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressCategoryFilterChange)
                return;

            RefreshProducts();
        }

        private string GetSelectedCategoryFilter()
        {
            if (cmbProductCategory.SelectedIndex <= 0)
                return null;

            return cmbProductCategory.SelectedItem?.ToString();
        }

        private void ResetProductFilters()
        {
            txtSearch.Clear();
            chkLowStockOnly.Checked = false;
            _suppressCategoryFilterChange = true;
            cmbProductCategory.SelectedIndex = 0;
            _suppressCategoryFilterChange = false;
            RefreshProducts();
        }

        private void ResetCustomerFilters()
        {
            txtCustomerSearch.Clear();
            RefreshCustomers(preserveSelection: false);
        }

        private void ResetSupplierFilters()
        {
            txtSupplierSearch.Clear();
            RefreshSuppliers(preserveSelection: false);
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
            RefreshProductCategoryFilter();
            RefreshProducts();
            RefreshCustomers();
            RefreshSuppliers();
            RefreshInvoices();
            RefreshAccountsTab();
            RefreshProduction();
            RefreshTransactions();
            UpdateActionButtons();
            UpdateCustomerButtons();
            UpdateSupplierButtons();
            UpdateInvoiceButtons();
            UpdateProductionButtons();
        }

        private void RefreshProducts(bool preserveRowSelection = true)
        {
            var selectedId = preserveRowSelection ? GetSelectedProduct()?.Id : null;

            dgvProducts.Rows.Clear();

            var products = _repository.SearchProducts(
                txtSearch.Text,
                GetActiveInventoryProductType(),
                GetSelectedCategoryFilter());
            if (chkLowStockOnly.Checked)
                products = products.Where(p => p.IsLowStock);

            var productList = products.ToList();
            lblInventoryTotalValue.Text = $"Total value: {productList.Sum(p => p.StockValue):C2}";

            _suppressFgCheckboxEvents = true;
            try
            {
                foreach (var p in productList)
                {
                    var idx = dgvProducts.Rows.Add(
                        _fgSelectedProductIds.Contains(p.Id),
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
            }
            finally
            {
                _suppressFgCheckboxEvents = false;
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

            UpdateSelectAllHeaderState();
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

        private void RefreshSuppliers(bool preserveSelection = true)
        {
            var selectedId = preserveSelection ? GetSelectedSupplier()?.Id : null;
            dgvSuppliers.Rows.Clear();

            foreach (var s in _repository.SearchSuppliers(txtSupplierSearch.Text))
            {
                var idx = dgvSuppliers.Rows.Add(s.Name, s.Phone, s.Email, s.Address);
                dgvSuppliers.Rows[idx].Tag = s;
            }

            if (selectedId.HasValue)
            {
                var reselected = false;
                foreach (DataGridViewRow row in dgvSuppliers.Rows)
                {
                    if (row.Tag is Supplier supplier && supplier.Id == selectedId.Value)
                    {
                        _allowGridSelection = true;
                        row.Selected = true;
                        reselected = true;
                        break;
                    }
                }

                if (!reselected)
                    ApplyNoSelection(dgvSuppliers);
            }
            else
            {
                ApplyNoSelection(dgvSuppliers);
            }

            UpdateSupplierButtons();
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
                    invoice.AmountPaid,
                    invoice.BalanceDue,
                    invoice.IsActive ? "Active" : "Voided",
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
                var idx = dgvProductionOrders.Rows.Add(
                    order.Timestamp,
                    order.ProductionNumber,
                    order.RecipeName,
                    order.OutputProductSku,
                    order.OutputProductName,
                    order.QuantityProduced,
                    order.TotalOutputValue,
                    order.IsActive ? "Active" : "Reversed",
                    order.Notes);
                dgvProductionOrders.Rows[idx].Tag = order;
            }

            ApplyNoSelection(dgvProductionOrders);
            UpdateProductionHistoryButtons();
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
            if (dgvProducts.SelectedRows.Count == 0)
                return null;

            return dgvProducts.SelectedRows[0].Tag as Product;
        }

        private List<Product> GetCheckedProducts()
        {
            return GetFgCartProducts();
        }

        private List<Product> GetSelectedProducts()
        {
            if (IsFgInventoryTab)
                return GetFgCartProducts();

            return GetRawMaterialCartProducts();
        }

        private Customer GetSelectedCustomer()
        {
            if (dgvCustomers.SelectedRows.Count == 0) return null;
            return dgvCustomers.SelectedRows[0].Tag as Customer;
        }

        private Supplier GetSelectedSupplier()
        {
            if (dgvSuppliers.SelectedRows.Count == 0) return null;
            return dgvSuppliers.SelectedRows[0].Tag as Supplier;
        }

        private Invoice GetSelectedInvoice()
        {
            if (dgvInvoices.SelectedRows.Count == 0) return null;
            return dgvInvoices.SelectedRows[0].Tag as Invoice;
        }

        private ProductionOrder GetSelectedProductionOrder()
        {
            if (dgvProductionOrders.SelectedRows.Count == 0) return null;
            return dgvProductionOrders.SelectedRows[0].Tag as ProductionOrder;
        }

        private ProductionRecipe GetSelectedRecipe()
        {
            if (dgvRecipes.SelectedRows.Count == 0) return null;
            return dgvRecipes.SelectedRows[0].Tag as ProductionRecipe;
        }

        private void UpdateActionButtons()
        {
            var isFg = IsFgInventoryTab;
            var rowProduct = GetSelectedProduct();
            var cartCount = isFg ? GetFgCartProducts().Count : GetRawMaterialCartProducts().Count;

            btnEdit.Enabled = rowProduct != null;
            btnDelete.Enabled = rowProduct != null;
            btnStockIn.Visible = !isFg;
            btnStockOut.Visible = isFg;
            btnStockIn.Enabled = !isFg && cartCount > 0;
            btnStockOut.Enabled = isFg && cartCount > 0;
        }

        private void UpdateCustomerButtons()
        {
            var hasSelection = GetSelectedCustomer() != null;
            btnEditCustomer.Enabled = hasSelection;
            btnDeleteCustomer.Enabled = hasSelection;
        }

        private void UpdateSupplierButtons()
        {
            var hasSelection = GetSelectedSupplier() != null;
            btnEditSupplier.Enabled = hasSelection;
            btnDeleteSupplier.Enabled = hasSelection;
        }

        private void UpdateInvoiceButtons()
        {
            var invoice = GetSelectedInvoice();
            var hasSelection = invoice != null;
            var isActive = invoice?.IsActive ?? false;
            btnViewInvoice.Enabled = hasSelection;
            btnPrintInvoice.Enabled = hasSelection;
            btnChalan.Enabled = hasSelection;
            btnVoidInvoice.Enabled = hasSelection && isActive;
            btnRestoreSale.Enabled = hasSelection && !isActive;
        }

        private void UpdateProductionHistoryButtons()
        {
            var order = GetSelectedProductionOrder();
            var hasSelection = order != null;
            var isActive = order?.IsActive ?? false;
            btnReverseProduction.Enabled = hasSelection && isActive;
            btnRestoreProduction.Enabled = hasSelection && !isActive;
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
                MessageBox.Show("Select a production first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                $"Delete production \"{recipe.Name}\"?",
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
                MessageBox.Show("Select a production first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void BtnAddSupplier_Click(object sender, EventArgs e)
        {
            using (var form = new SupplierEditForm(_repository))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    RefreshAll();
            }
        }

        private void BtnEditSupplier_Click(object sender, EventArgs e)
        {
            var supplier = GetSelectedSupplier();
            if (supplier == null)
            {
                MessageBox.Show("Select a supplier first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new SupplierEditForm(_repository, supplier))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    RefreshAll();
            }
        }

        private void BtnDeleteSupplier_Click(object sender, EventArgs e)
        {
            var supplier = GetSelectedSupplier();
            if (supplier == null) return;

            var confirm = MessageBox.Show(
                $"Delete supplier \"{supplier.Name}\"?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            _repository.DeleteSupplier(supplier.Id);
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

        private void BtnChalan_Click(object sender, EventArgs e)
        {
            var invoice = GetSelectedInvoice();
            if (invoice == null)
            {
                MessageBox.Show("Select an invoice first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var latest = _repository.GetInvoice(invoice.Id) ?? invoice;
            using (var form = new ChalanForm(latest))
                form.ShowDialog(this);
        }

        private void BtnVoidInvoice_Click(object sender, EventArgs e)
        {
            var invoice = GetSelectedInvoice();
            if (invoice == null)
                return;

            var latest = _repository.GetInvoice(invoice.Id) ?? invoice;
            if (!latest.IsActive)
            {
                MessageBox.Show(
                    "This sale is already voided. Use Restore Sale first if you need to undo the void.",
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                RefreshAll();
                return;
            }

            var confirm = MessageBox.Show(
                $"Void sale {latest.InvoiceNumber}?\n\n" +
                "This will:\n" +
                "- Return sold stock to inventory\n" +
                "- Reverse accounts (AR and Sales Revenue)\n" +
                "- Reverse any payment recorded on this invoice\n\n" +
                "After voiding, fix production/materials and sell again with a new invoice.",
                "Void Sale",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            var error = _repository.VoidInvoice(latest.Id, "Correcting mistake");
            if (error != null)
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RefreshAll();
            MessageBox.Show(
                "Sale voided. Next: reverse the wrong production run, fix materials/recipe, then re-run production and sell again.",
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void BtnRestoreSale_Click(object sender, EventArgs e)
        {
            var invoice = GetSelectedInvoice();
            if (invoice == null)
                return;

            var latest = _repository.GetInvoice(invoice.Id) ?? invoice;
            if (latest.IsActive)
            {
                MessageBox.Show(
                    "This sale is already active. Only voided sales can be restored.",
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                RefreshAll();
                return;
            }

            var confirm = MessageBox.Show(
                $"Restore voided sale {latest.InvoiceNumber}?\n\n" +
                "This will undo the void and put the sale back into stock, accounts, and reports.",
                "Restore Sale",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            var error = _repository.RestoreInvoice(latest.Id, "Undo mistaken void");
            if (error != null)
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RefreshAll();
        }

        private void BtnReverseProduction_Click(object sender, EventArgs e)
        {
            var order = GetSelectedProductionOrder();
            if (order == null || !order.IsActive)
                return;

            var confirm = MessageBox.Show(
                $"Reverse {order.ProductionNumber}?\n\n" +
                "This will:\n" +
                "- Remove produced finished goods from stock\n" +
                "- Return raw materials used in that run\n" +
                "- Restore product unit cost from before that run\n\n" +
                "If these goods were already sold, void the sale first.",
                "Reverse Production",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            var error = _repository.ReverseProduction(order.Id, "Correcting mistake");
            if (error != null)
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RefreshAll();
            MessageBox.Show(
                "Production reversed. Fix raw material price/recipe, then run production and sell again.",
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void BtnRestoreProduction_Click(object sender, EventArgs e)
        {
            var order = GetSelectedProductionOrder();
            if (order == null || order.IsActive)
                return;

            var confirm = MessageBox.Show(
                $"Restore reversed production {order.ProductionNumber}?\n\n" +
                "This will undo the reversal and put materials and finished goods back as they were.",
                "Restore Production",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            var error = _repository.RestoreProduction(order.Id, "Undo mistaken reversal");
            if (error != null)
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RefreshAll();
        }

        private void BtnCorrectionGuide_Click(object sender, EventArgs e)
        {
            using (var form = new CorrectionGuideForm())
                form.ShowDialog(this);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new ProductEditForm(_repository, null, GetActiveInventoryProductType()))
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
            _fgSelectedProductIds.Remove(product.Id);
            RefreshAll();
        }

        private void OpenStockIn()
        {
            var products = GetRawMaterialCartProducts();
            if (products.Count == 0)
            {
                MessageBox.Show("Select at least one raw material.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var latest = products
                .Select(p => _repository.GetProduct(p.Id) ?? p)
                .ToList();

            using (var form = new MultiStockInForm(_repository, latest))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    ClearInventoryProductSelection();
                    RefreshAll();
                }
            }
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

        private void OpenStockOut()
        {
            var products = GetSelectedProducts();
            if (products.Count == 0)
            {
                MessageBox.Show("Select at least one product.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var latest = products
                .Select(p => _repository.GetProduct(p.Id) ?? p)
                .ToList();

            using (var form = new MultiStockOutForm(_repository, latest))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    ClearFgProductSelection();
                    RefreshAll();
                }
            }
        }

        private void RefreshSettingsTab()
        {
            lblSoftwareProductValue.Text = AppVersion.ProductName;
            lblSoftwareVersionValue.Text = AppVersion.DisplayVersion;
            lblDeveloperNameValue.Text = DeveloperInfo.Name;
            lblDeveloperEmailValue.Text = DeveloperInfo.Email;
            lblDeveloperMobileValue.Text = DeveloperInfo.Mobile;
            LoadDeveloperPhoto();

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

        private void LoadDeveloperPhoto()
        {
            var previous = pbDeveloperPhoto.Image;
            pbDeveloperPhoto.Image = DeveloperInfo.LoadPhoto();
            previous?.Dispose();
        }

        private void BtnBackupDatabase_Click(object sender, EventArgs e)
        {
            var defaultFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Electronics Backups");

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
