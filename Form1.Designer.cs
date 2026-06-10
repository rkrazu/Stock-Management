namespace Stock_Managemnet
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStats;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabInventory;
        private System.Windows.Forms.TabPage tabCustomers;
        private System.Windows.Forms.TabPage tabInvoices;
        private System.Windows.Forms.TabPage tabProduction;
        private System.Windows.Forms.TabPage tabTransactions;
        private System.Windows.Forms.TableLayoutPanel tlpInventory;
        private System.Windows.Forms.TableLayoutPanel tlpCustomers;
        private System.Windows.Forms.Panel panelInventoryToolbar;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.CheckBox chkLowStockOnly;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnStockIn;
        private System.Windows.Forms.Button btnStockOut;
        private System.Windows.Forms.DataGridView dgvProducts;
        private System.Windows.Forms.Panel panelCustomerToolbar;
        private System.Windows.Forms.TextBox txtCustomerSearch;
        private System.Windows.Forms.Button btnCustomerSearch;
        private System.Windows.Forms.Button btnCustomerReset;
        private System.Windows.Forms.Button btnAddCustomer;
        private System.Windows.Forms.Button btnEditCustomer;
        private System.Windows.Forms.Button btnDeleteCustomer;
        private System.Windows.Forms.DataGridView dgvCustomers;
        private System.Windows.Forms.TableLayoutPanel tlpInvoices;
        private System.Windows.Forms.Panel panelInvoiceToolbar;
        private System.Windows.Forms.TextBox txtInvoiceSearch;
        private System.Windows.Forms.Button btnInvoiceSearch;
        private System.Windows.Forms.Button btnInvoiceReset;
        private System.Windows.Forms.Button btnViewInvoice;
        private System.Windows.Forms.Button btnPrintInvoice;
        private System.Windows.Forms.DataGridView dgvInvoices;
        private System.Windows.Forms.TableLayoutPanel tlpProduction;
        private System.Windows.Forms.Panel panelProductionToolbar;
        private System.Windows.Forms.TextBox txtProductionSearch;
        private System.Windows.Forms.Button btnProductionSearch;
        private System.Windows.Forms.Button btnProductionReset;
        private System.Windows.Forms.Button btnAddRecipe;
        private System.Windows.Forms.Button btnEditRecipe;
        private System.Windows.Forms.Button btnDeleteRecipe;
        private System.Windows.Forms.Button btnRunProduction;
        private System.Windows.Forms.Label lblRecipes;
        private System.Windows.Forms.DataGridView dgvRecipes;
        private System.Windows.Forms.Label lblProductionHistory;
        private System.Windows.Forms.DataGridView dgvProductionOrders;
        private System.Windows.Forms.TableLayoutPanel tlpTransactions;
        private System.Windows.Forms.Panel panelTransactionToolbar;
        private System.Windows.Forms.TextBox txtTxnSearch;
        private System.Windows.Forms.Button btnTxnSearch;
        private System.Windows.Forms.Button btnTxnReset;
        private System.Windows.Forms.Button btnFilterIn;
        private System.Windows.Forms.Button btnFilterOut;
        private System.Windows.Forms.Label lblTxnFrom;
        private System.Windows.Forms.DateTimePicker dtpTxnFrom;
        private System.Windows.Forms.Label lblTxnTo;
        private System.Windows.Forms.DateTimePicker dtpTxnTo;
        private System.Windows.Forms.CheckBox chkTxnDateRange;
        private System.Windows.Forms.DataGridView dgvTransactions;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStats = new System.Windows.Forms.Label();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabInventory = new System.Windows.Forms.TabPage();
            this.tlpInventory = new System.Windows.Forms.TableLayoutPanel();
            this.dgvProducts = new System.Windows.Forms.DataGridView();
            this.panelInventoryToolbar = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.chkLowStockOnly = new System.Windows.Forms.CheckBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnStockIn = new System.Windows.Forms.Button();
            this.btnStockOut = new System.Windows.Forms.Button();
            this.tabCustomers = new System.Windows.Forms.TabPage();
            this.tlpCustomers = new System.Windows.Forms.TableLayoutPanel();
            this.dgvCustomers = new System.Windows.Forms.DataGridView();
            this.panelCustomerToolbar = new System.Windows.Forms.Panel();
            this.txtCustomerSearch = new System.Windows.Forms.TextBox();
            this.btnCustomerSearch = new System.Windows.Forms.Button();
            this.btnCustomerReset = new System.Windows.Forms.Button();
            this.btnAddCustomer = new System.Windows.Forms.Button();
            this.btnEditCustomer = new System.Windows.Forms.Button();
            this.btnDeleteCustomer = new System.Windows.Forms.Button();
            this.tabInvoices = new System.Windows.Forms.TabPage();
            this.tlpInvoices = new System.Windows.Forms.TableLayoutPanel();
            this.dgvInvoices = new System.Windows.Forms.DataGridView();
            this.panelInvoiceToolbar = new System.Windows.Forms.Panel();
            this.txtInvoiceSearch = new System.Windows.Forms.TextBox();
            this.btnInvoiceSearch = new System.Windows.Forms.Button();
            this.btnInvoiceReset = new System.Windows.Forms.Button();
            this.btnViewInvoice = new System.Windows.Forms.Button();
            this.btnPrintInvoice = new System.Windows.Forms.Button();
            this.tabProduction = new System.Windows.Forms.TabPage();
            this.tlpProduction = new System.Windows.Forms.TableLayoutPanel();
            this.panelProductionToolbar = new System.Windows.Forms.Panel();
            this.txtProductionSearch = new System.Windows.Forms.TextBox();
            this.btnProductionSearch = new System.Windows.Forms.Button();
            this.btnProductionReset = new System.Windows.Forms.Button();
            this.btnAddRecipe = new System.Windows.Forms.Button();
            this.btnEditRecipe = new System.Windows.Forms.Button();
            this.btnDeleteRecipe = new System.Windows.Forms.Button();
            this.btnRunProduction = new System.Windows.Forms.Button();
            this.lblRecipes = new System.Windows.Forms.Label();
            this.dgvRecipes = new System.Windows.Forms.DataGridView();
            this.lblProductionHistory = new System.Windows.Forms.Label();
            this.dgvProductionOrders = new System.Windows.Forms.DataGridView();
            this.tabTransactions = new System.Windows.Forms.TabPage();
            this.tlpTransactions = new System.Windows.Forms.TableLayoutPanel();
            this.dgvTransactions = new System.Windows.Forms.DataGridView();
            this.panelTransactionToolbar = new System.Windows.Forms.Panel();
            this.txtTxnSearch = new System.Windows.Forms.TextBox();
            this.btnTxnSearch = new System.Windows.Forms.Button();
            this.btnTxnReset = new System.Windows.Forms.Button();
            this.btnFilterIn = new System.Windows.Forms.Button();
            this.btnFilterOut = new System.Windows.Forms.Button();
            this.lblTxnFrom = new System.Windows.Forms.Label();
            this.dtpTxnFrom = new System.Windows.Forms.DateTimePicker();
            this.lblTxnTo = new System.Windows.Forms.Label();
            this.dtpTxnTo = new System.Windows.Forms.DateTimePicker();
            this.chkTxnDateRange = new System.Windows.Forms.CheckBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelHeader.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabInventory.SuspendLayout();
            this.tlpInventory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).BeginInit();
            this.panelInventoryToolbar.SuspendLayout();
            this.tabCustomers.SuspendLayout();
            this.tlpCustomers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).BeginInit();
            this.panelCustomerToolbar.SuspendLayout();
            this.tabInvoices.SuspendLayout();
            this.tlpInvoices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).BeginInit();
            this.panelInvoiceToolbar.SuspendLayout();
            this.tabProduction.SuspendLayout();
            this.tlpProduction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecipes)).BeginInit();
            this.panelProductionToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductionOrders)).BeginInit();
            this.tabTransactions.SuspendLayout();
            this.tlpTransactions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransactions)).BeginInit();
            this.panelTransactionToolbar.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Height = 72;
            this.panelHeader.Controls.Add(this.lblStats);
            this.panelHeader.Controls.Add(this.lblTitle);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTitle.Text = "Stock Management";
            //
            // lblStats
            //
            this.lblStats.AutoSize = true;
            this.lblStats.ForeColor = System.Drawing.Color.FromArgb(219, 234, 254);
            this.lblStats.Location = new System.Drawing.Point(18, 44);
            this.lblStats.Text = "Loading...";
            //
            // tabMain
            //
            this.tabMain.Controls.Add(this.tabInventory);
            this.tabMain.Controls.Add(this.tabCustomers);
            this.tabMain.Controls.Add(this.tabInvoices);
            this.tabMain.Controls.Add(this.tabProduction);
            this.tabMain.Controls.Add(this.tabTransactions);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Font = new System.Drawing.Font("Segoe UI", 11F);
            //
            // tabInventory
            //
            this.tabInventory.Controls.Add(this.tlpInventory);
            this.tabInventory.Text = "Inventory";
            this.tabInventory.UseVisualStyleBackColor = true;
            //
            // tlpInventory — row 0 toolbar, row 1 grid (grid cannot overlap toolbar)
            //
            this.tlpInventory.ColumnCount = 1;
            this.tlpInventory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInventory.Controls.Add(this.panelInventoryToolbar, 0, 0);
            this.tlpInventory.Controls.Add(this.dgvProducts, 0, 1);
            this.tlpInventory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpInventory.RowCount = 2;
            this.tlpInventory.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tlpInventory.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            //
            // panelInventoryToolbar
            //
            this.panelInventoryToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelInventoryToolbar.Padding = new System.Windows.Forms.Padding(8, 8, 8, 4);
            this.panelInventoryToolbar.Controls.Add(this.txtSearch);
            this.panelInventoryToolbar.Controls.Add(this.btnSearch);
            this.panelInventoryToolbar.Controls.Add(this.btnReset);
            this.panelInventoryToolbar.Controls.Add(this.chkLowStockOnly);
            this.panelInventoryToolbar.Controls.Add(this.btnAdd);
            this.panelInventoryToolbar.Controls.Add(this.btnEdit);
            this.panelInventoryToolbar.Controls.Add(this.btnDelete);
            this.panelInventoryToolbar.Controls.Add(this.btnStockIn);
            this.panelInventoryToolbar.Controls.Add(this.btnStockOut);
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(11, 12);
            this.txtSearch.Size = new System.Drawing.Size(220, 23);
            //
            // btnSearch
            //
            this.btnSearch.Location = new System.Drawing.Point(237, 10);
            this.btnSearch.Size = new System.Drawing.Size(65, 27);
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            //
            // btnReset
            //
            this.btnReset.Location = new System.Drawing.Point(308, 10);
            this.btnReset.Size = new System.Drawing.Size(65, 27);
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            //
            // chkLowStockOnly
            //
            this.chkLowStockOnly.AutoSize = true;
            this.chkLowStockOnly.Location = new System.Drawing.Point(385, 12);
            this.chkLowStockOnly.Text = "Low stock only";
            this.chkLowStockOnly.UseVisualStyleBackColor = true;
            //
            // btnAdd
            //
            this.btnAdd.Location = new System.Drawing.Point(505, 10);
            this.btnAdd.Size = new System.Drawing.Size(90, 27);
            this.btnAdd.Text = "+ Add Product";
            this.btnAdd.UseVisualStyleBackColor = true;
            //
            // btnEdit
            //
            this.btnEdit.Location = new System.Drawing.Point(601, 10);
            this.btnEdit.Size = new System.Drawing.Size(75, 27);
            this.btnEdit.Text = "Edit";
            this.btnEdit.Enabled = false;
            //
            // btnDelete
            //
            this.btnDelete.Location = new System.Drawing.Point(682, 10);
            this.btnDelete.Size = new System.Drawing.Size(75, 27);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Enabled = false;
            //
            // btnStockIn
            //
            this.btnStockIn.Location = new System.Drawing.Point(763, 10);
            this.btnStockIn.Size = new System.Drawing.Size(85, 27);
            this.btnStockIn.Text = "Stock In";
            this.btnStockIn.Enabled = false;
            //
            // btnStockOut
            //
            this.btnStockOut.Location = new System.Drawing.Point(854, 10);
            this.btnStockOut.Size = new System.Drawing.Size(85, 27);
            this.btnStockOut.Text = "Stock Out";
            this.btnStockOut.Enabled = false;
            //
            // dgvProducts
            //
            this.dgvProducts.AllowUserToAddRows = false;
            this.dgvProducts.AllowUserToDeleteRows = false;
            this.dgvProducts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProducts.BackgroundColor = System.Drawing.Color.White;
            this.dgvProducts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProducts.MultiSelect = false;
            this.dgvProducts.ReadOnly = true;
            this.dgvProducts.RowHeadersVisible = false;
            this.dgvProducts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            //
            // tabCustomers
            //
            this.tabCustomers.Controls.Add(this.tlpCustomers);
            this.tabCustomers.Text = "Customers";
            this.tabCustomers.UseVisualStyleBackColor = true;
            //
            // tlpCustomers
            //
            this.tlpCustomers.ColumnCount = 1;
            this.tlpCustomers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCustomers.Controls.Add(this.panelCustomerToolbar, 0, 0);
            this.tlpCustomers.Controls.Add(this.dgvCustomers, 0, 1);
            this.tlpCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCustomers.RowCount = 2;
            this.tlpCustomers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tlpCustomers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            //
            // panelCustomerToolbar
            //
            this.panelCustomerToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCustomerToolbar.Padding = new System.Windows.Forms.Padding(8, 8, 8, 4);
            this.panelCustomerToolbar.Controls.Add(this.txtCustomerSearch);
            this.panelCustomerToolbar.Controls.Add(this.btnCustomerSearch);
            this.panelCustomerToolbar.Controls.Add(this.btnCustomerReset);
            this.panelCustomerToolbar.Controls.Add(this.btnAddCustomer);
            this.panelCustomerToolbar.Controls.Add(this.btnEditCustomer);
            this.panelCustomerToolbar.Controls.Add(this.btnDeleteCustomer);
            //
            // txtCustomerSearch
            //
            this.txtCustomerSearch.Location = new System.Drawing.Point(11, 12);
            this.txtCustomerSearch.Size = new System.Drawing.Size(220, 23);
            //
            // btnCustomerSearch
            //
            this.btnCustomerSearch.Location = new System.Drawing.Point(237, 10);
            this.btnCustomerSearch.Size = new System.Drawing.Size(65, 27);
            this.btnCustomerSearch.Text = "Search";
            this.btnCustomerSearch.UseVisualStyleBackColor = true;
            //
            // btnCustomerReset
            //
            this.btnCustomerReset.Location = new System.Drawing.Point(308, 10);
            this.btnCustomerReset.Size = new System.Drawing.Size(65, 27);
            this.btnCustomerReset.Text = "Reset";
            this.btnCustomerReset.UseVisualStyleBackColor = true;
            //
            // btnAddCustomer
            //
            this.btnAddCustomer.Location = new System.Drawing.Point(385, 10);
            this.btnAddCustomer.Size = new System.Drawing.Size(105, 27);
            this.btnAddCustomer.Text = "+ Add Customer";
            this.btnAddCustomer.UseVisualStyleBackColor = true;
            //
            // btnEditCustomer
            //
            this.btnEditCustomer.Location = new System.Drawing.Point(496, 10);
            this.btnEditCustomer.Size = new System.Drawing.Size(75, 27);
            this.btnEditCustomer.Text = "Edit";
            this.btnEditCustomer.Enabled = false;
            //
            // btnDeleteCustomer
            //
            this.btnDeleteCustomer.Location = new System.Drawing.Point(577, 10);
            this.btnDeleteCustomer.Size = new System.Drawing.Size(75, 27);
            this.btnDeleteCustomer.Text = "Delete";
            this.btnDeleteCustomer.Enabled = false;
            //
            // dgvCustomers
            //
            this.dgvCustomers.AllowUserToAddRows = false;
            this.dgvCustomers.AllowUserToDeleteRows = false;
            this.dgvCustomers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCustomers.BackgroundColor = System.Drawing.Color.White;
            this.dgvCustomers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCustomers.MultiSelect = false;
            this.dgvCustomers.ReadOnly = true;
            this.dgvCustomers.RowHeadersVisible = false;
            this.dgvCustomers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            //
            // tabInvoices
            //
            this.tabInvoices.Controls.Add(this.tlpInvoices);
            this.tabInvoices.Text = "Invoices";
            this.tabInvoices.UseVisualStyleBackColor = true;
            //
            // tlpInvoices
            //
            this.tlpInvoices.ColumnCount = 1;
            this.tlpInvoices.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInvoices.Controls.Add(this.panelInvoiceToolbar, 0, 0);
            this.tlpInvoices.Controls.Add(this.dgvInvoices, 0, 1);
            this.tlpInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpInvoices.RowCount = 2;
            this.tlpInvoices.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tlpInvoices.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            //
            // panelInvoiceToolbar
            //
            this.panelInvoiceToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelInvoiceToolbar.Padding = new System.Windows.Forms.Padding(8, 8, 8, 4);
            this.panelInvoiceToolbar.Controls.Add(this.txtInvoiceSearch);
            this.panelInvoiceToolbar.Controls.Add(this.btnInvoiceSearch);
            this.panelInvoiceToolbar.Controls.Add(this.btnInvoiceReset);
            this.panelInvoiceToolbar.Controls.Add(this.btnViewInvoice);
            this.panelInvoiceToolbar.Controls.Add(this.btnPrintInvoice);
            //
            // txtInvoiceSearch
            //
            this.txtInvoiceSearch.Location = new System.Drawing.Point(11, 10);
            this.txtInvoiceSearch.Size = new System.Drawing.Size(220, 23);
            //
            // btnInvoiceSearch
            //
            this.btnInvoiceSearch.Location = new System.Drawing.Point(237, 10);
            this.btnInvoiceSearch.Size = new System.Drawing.Size(65, 27);
            this.btnInvoiceSearch.Text = "Search";
            this.btnInvoiceSearch.UseVisualStyleBackColor = true;
            //
            // btnInvoiceReset
            //
            this.btnInvoiceReset.Location = new System.Drawing.Point(308, 10);
            this.btnInvoiceReset.Size = new System.Drawing.Size(65, 27);
            this.btnInvoiceReset.Text = "Reset";
            this.btnInvoiceReset.UseVisualStyleBackColor = true;
            //
            // btnViewInvoice
            //
            this.btnViewInvoice.Location = new System.Drawing.Point(385, 10);
            this.btnViewInvoice.Size = new System.Drawing.Size(110, 27);
            this.btnViewInvoice.Text = "View Invoice";
            this.btnViewInvoice.Enabled = false;
            this.btnViewInvoice.UseVisualStyleBackColor = true;
            //
            // btnPrintInvoice
            //
            this.btnPrintInvoice.Location = new System.Drawing.Point(501, 10);
            this.btnPrintInvoice.Size = new System.Drawing.Size(110, 27);
            this.btnPrintInvoice.Text = "Print";
            this.btnPrintInvoice.Enabled = false;
            this.btnPrintInvoice.UseVisualStyleBackColor = true;
            //
            // dgvInvoices
            //
            this.dgvInvoices.AllowUserToAddRows = false;
            this.dgvInvoices.AllowUserToDeleteRows = false;
            this.dgvInvoices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInvoices.BackgroundColor = System.Drawing.Color.White;
            this.dgvInvoices.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvInvoices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInvoices.MultiSelect = false;
            this.dgvInvoices.ReadOnly = true;
            this.dgvInvoices.RowHeadersVisible = false;
            this.dgvInvoices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            //
            // tabProduction
            //
            this.tabProduction.Controls.Add(this.tlpProduction);
            this.tabProduction.Text = "Production";
            this.tabProduction.UseVisualStyleBackColor = true;
            //
            // tlpProduction
            //
            this.tlpProduction.ColumnCount = 1;
            this.tlpProduction.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpProduction.Controls.Add(this.panelProductionToolbar, 0, 0);
            this.tlpProduction.Controls.Add(this.lblRecipes, 0, 1);
            this.tlpProduction.Controls.Add(this.dgvRecipes, 0, 2);
            this.tlpProduction.Controls.Add(this.lblProductionHistory, 0, 3);
            this.tlpProduction.Controls.Add(this.dgvProductionOrders, 0, 4);
            this.tlpProduction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpProduction.RowCount = 5;
            this.tlpProduction.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tlpProduction.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpProduction.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tlpProduction.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpProduction.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            //
            // panelProductionToolbar
            //
            this.panelProductionToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelProductionToolbar.Padding = new System.Windows.Forms.Padding(8, 8, 8, 4);
            this.panelProductionToolbar.Controls.Add(this.txtProductionSearch);
            this.panelProductionToolbar.Controls.Add(this.btnProductionSearch);
            this.panelProductionToolbar.Controls.Add(this.btnProductionReset);
            this.panelProductionToolbar.Controls.Add(this.btnAddRecipe);
            this.panelProductionToolbar.Controls.Add(this.btnEditRecipe);
            this.panelProductionToolbar.Controls.Add(this.btnDeleteRecipe);
            this.panelProductionToolbar.Controls.Add(this.btnRunProduction);
            //
            // txtProductionSearch
            //
            this.txtProductionSearch.Location = new System.Drawing.Point(11, 12);
            this.txtProductionSearch.Size = new System.Drawing.Size(220, 23);
            //
            // btnProductionSearch
            //
            this.btnProductionSearch.Location = new System.Drawing.Point(237, 10);
            this.btnProductionSearch.Size = new System.Drawing.Size(65, 27);
            this.btnProductionSearch.Text = "Search";
            this.btnProductionSearch.UseVisualStyleBackColor = true;
            //
            // btnProductionReset
            //
            this.btnProductionReset.Location = new System.Drawing.Point(308, 10);
            this.btnProductionReset.Size = new System.Drawing.Size(65, 27);
            this.btnProductionReset.Text = "Reset";
            this.btnProductionReset.UseVisualStyleBackColor = true;
            //
            // btnAddRecipe
            //
            this.btnAddRecipe.Location = new System.Drawing.Point(390, 10);
            this.btnAddRecipe.Size = new System.Drawing.Size(95, 27);
            this.btnAddRecipe.Text = "+ Add Recipe";
            this.btnAddRecipe.UseVisualStyleBackColor = true;
            //
            // btnEditRecipe
            //
            this.btnEditRecipe.Location = new System.Drawing.Point(491, 10);
            this.btnEditRecipe.Size = new System.Drawing.Size(75, 27);
            this.btnEditRecipe.Text = "Edit";
            this.btnEditRecipe.Enabled = false;
            //
            // btnDeleteRecipe
            //
            this.btnDeleteRecipe.Location = new System.Drawing.Point(572, 10);
            this.btnDeleteRecipe.Size = new System.Drawing.Size(75, 27);
            this.btnDeleteRecipe.Text = "Delete";
            this.btnDeleteRecipe.Enabled = false;
            //
            // btnRunProduction
            //
            this.btnRunProduction.Location = new System.Drawing.Point(653, 10);
            this.btnRunProduction.Size = new System.Drawing.Size(105, 27);
            this.btnRunProduction.Text = "Run Production";
            this.btnRunProduction.Enabled = false;
            //
            // lblRecipes
            //
            this.lblRecipes.AutoSize = true;
            this.lblRecipes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRecipes.Padding = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.lblRecipes.Text = "Recipes";
            //
            // dgvRecipes
            //
            this.dgvRecipes.AllowUserToAddRows = false;
            this.dgvRecipes.AllowUserToDeleteRows = false;
            this.dgvRecipes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRecipes.BackgroundColor = System.Drawing.Color.White;
            this.dgvRecipes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvRecipes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecipes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecipes.MultiSelect = false;
            this.dgvRecipes.ReadOnly = true;
            this.dgvRecipes.RowHeadersVisible = false;
            this.dgvRecipes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            //
            // lblProductionHistory
            //
            this.lblProductionHistory.AutoSize = true;
            this.lblProductionHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProductionHistory.Padding = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.lblProductionHistory.Text = "Production History";
            //
            // dgvProductionOrders
            //
            this.dgvProductionOrders.AllowUserToAddRows = false;
            this.dgvProductionOrders.AllowUserToDeleteRows = false;
            this.dgvProductionOrders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProductionOrders.BackgroundColor = System.Drawing.Color.White;
            this.dgvProductionOrders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvProductionOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductionOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProductionOrders.MultiSelect = false;
            this.dgvProductionOrders.ReadOnly = true;
            this.dgvProductionOrders.RowHeadersVisible = false;
            this.dgvProductionOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            //
            // tabTransactions
            //
            this.tabTransactions.Controls.Add(this.tlpTransactions);
            this.tabTransactions.Text = "Transaction History";
            //
            // tlpTransactions
            //
            this.tlpTransactions.ColumnCount = 1;
            this.tlpTransactions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTransactions.Controls.Add(this.panelTransactionToolbar, 0, 0);
            this.tlpTransactions.Controls.Add(this.dgvTransactions, 0, 1);
            this.tlpTransactions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTransactions.RowCount = 2;
            this.tlpTransactions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tlpTransactions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            //
            // panelTransactionToolbar
            //
            this.panelTransactionToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTransactionToolbar.Padding = new System.Windows.Forms.Padding(8, 8, 8, 4);
            this.panelTransactionToolbar.Controls.Add(this.txtTxnSearch);
            this.panelTransactionToolbar.Controls.Add(this.btnTxnSearch);
            this.panelTransactionToolbar.Controls.Add(this.btnTxnReset);
            this.panelTransactionToolbar.Controls.Add(this.btnFilterIn);
            this.panelTransactionToolbar.Controls.Add(this.btnFilterOut);
            this.panelTransactionToolbar.Controls.Add(this.lblTxnFrom);
            this.panelTransactionToolbar.Controls.Add(this.dtpTxnFrom);
            this.panelTransactionToolbar.Controls.Add(this.lblTxnTo);
            this.panelTransactionToolbar.Controls.Add(this.dtpTxnTo);
            this.panelTransactionToolbar.Controls.Add(this.chkTxnDateRange);
            //
            // txtTxnSearch
            //
            this.txtTxnSearch.Location = new System.Drawing.Point(11, 12);
            this.txtTxnSearch.Size = new System.Drawing.Size(200, 23);
            //
            // btnTxnSearch
            //
            this.btnTxnSearch.Location = new System.Drawing.Point(217, 10);
            this.btnTxnSearch.Size = new System.Drawing.Size(65, 27);
            this.btnTxnSearch.Text = "Search";
            this.btnTxnSearch.UseVisualStyleBackColor = true;
            //
            // btnTxnReset
            //
            this.btnTxnReset.Location = new System.Drawing.Point(288, 10);
            this.btnTxnReset.Size = new System.Drawing.Size(65, 27);
            this.btnTxnReset.Text = "Reset";
            this.btnTxnReset.UseVisualStyleBackColor = true;
            //
            // btnFilterIn
            //
            this.btnFilterIn.Location = new System.Drawing.Point(365, 10);
            this.btnFilterIn.Size = new System.Drawing.Size(50, 27);
            this.btnFilterIn.Text = "IN";
            this.btnFilterIn.UseVisualStyleBackColor = true;
            //
            // btnFilterOut
            //
            this.btnFilterOut.Location = new System.Drawing.Point(421, 10);
            this.btnFilterOut.Size = new System.Drawing.Size(55, 27);
            this.btnFilterOut.Text = "OUT";
            this.btnFilterOut.UseVisualStyleBackColor = true;
            //
            // lblTxnFrom
            //
            this.lblTxnFrom.AutoSize = true;
            this.lblTxnFrom.Location = new System.Drawing.Point(490, 14);
            this.lblTxnFrom.Text = "From:";
            //
            // dtpTxnFrom
            //
            this.dtpTxnFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTxnFrom.Location = new System.Drawing.Point(535, 11);
            this.dtpTxnFrom.Size = new System.Drawing.Size(105, 23);
            this.dtpTxnFrom.Enabled = false;
            //
            // lblTxnTo
            //
            this.lblTxnTo.AutoSize = true;
            this.lblTxnTo.Location = new System.Drawing.Point(648, 14);
            this.lblTxnTo.Text = "To:";
            //
            // dtpTxnTo
            //
            this.dtpTxnTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTxnTo.Location = new System.Drawing.Point(675, 11);
            this.dtpTxnTo.Size = new System.Drawing.Size(105, 23);
            this.dtpTxnTo.Enabled = false;
            //
            // chkTxnDateRange
            //
            this.chkTxnDateRange.AutoSize = true;
            this.chkTxnDateRange.Location = new System.Drawing.Point(790, 14);
            this.chkTxnDateRange.Text = "Date range";
            this.chkTxnDateRange.UseVisualStyleBackColor = true;
            //
            // dgvTransactions
            //
            this.dgvTransactions.AllowUserToAddRows = false;
            this.dgvTransactions.AllowUserToDeleteRows = false;
            this.dgvTransactions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTransactions.BackgroundColor = System.Drawing.Color.White;
            this.dgvTransactions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTransactions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTransactions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTransactions.MultiSelect = false;
            this.dgvTransactions.ReadOnly = true;
            this.dgvTransactions.RowHeadersVisible = false;
            this.dgvTransactions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.statusLabel });
            //
            // statusLabel
            //
            this.statusLabel.Spring = true;
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 620);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stock Management";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabInventory.ResumeLayout(false);
            this.tlpInventory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).EndInit();
            this.panelInventoryToolbar.ResumeLayout(false);
            this.panelInventoryToolbar.PerformLayout();
            this.tabCustomers.ResumeLayout(false);
            this.tlpCustomers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).EndInit();
            this.panelCustomerToolbar.ResumeLayout(false);
            this.panelCustomerToolbar.PerformLayout();
            this.tabInvoices.ResumeLayout(false);
            this.tlpInvoices.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).EndInit();
            this.panelInvoiceToolbar.ResumeLayout(false);
            this.panelInvoiceToolbar.PerformLayout();
            this.tabProduction.ResumeLayout(false);
            this.tlpProduction.ResumeLayout(false);
            this.tlpProduction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecipes)).EndInit();
            this.panelProductionToolbar.ResumeLayout(false);
            this.panelProductionToolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductionOrders)).EndInit();
            this.tabTransactions.ResumeLayout(false);
            this.tlpTransactions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransactions)).EndInit();
            this.panelTransactionToolbar.ResumeLayout(false);
            this.panelTransactionToolbar.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
