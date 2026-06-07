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
        private System.Windows.Forms.TabPage tabTransactions;
        private System.Windows.Forms.TableLayoutPanel tlpInventory;
        private System.Windows.Forms.Panel panelInventoryToolbar;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.CheckBox chkLowStockOnly;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnStockIn;
        private System.Windows.Forms.Button btnStockOut;
        private System.Windows.Forms.DataGridView dgvProducts;
        private System.Windows.Forms.DataGridView dgvTransactions;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuProducts;
        private System.Windows.Forms.ToolStripMenuItem menuAddProduct;
        private System.Windows.Forms.ToolStripMenuItem menuEditProduct;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteProduct;
        private System.Windows.Forms.ToolStripSeparator menuSep1;
        private System.Windows.Forms.ToolStripMenuItem menuStockIn;
        private System.Windows.Forms.ToolStripMenuItem menuStockOut;
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
            this.chkLowStockOnly = new System.Windows.Forms.CheckBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnStockIn = new System.Windows.Forms.Button();
            this.btnStockOut = new System.Windows.Forms.Button();
            this.tabTransactions = new System.Windows.Forms.TabPage();
            this.dgvTransactions = new System.Windows.Forms.DataGridView();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuProducts = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddProduct = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditProduct = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeleteProduct = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStockIn = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStockOut = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabInventory.SuspendLayout();
            this.tlpInventory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).BeginInit();
            this.panelInventoryToolbar.SuspendLayout();
            this.tabTransactions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransactions)).BeginInit();
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
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
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
            this.tabMain.Controls.Add(this.tabTransactions);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Font = new System.Drawing.Font("Segoe UI", 9F);
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
            // chkLowStockOnly
            //
            this.chkLowStockOnly.AutoSize = true;
            this.chkLowStockOnly.Location = new System.Drawing.Point(318, 14);
            this.chkLowStockOnly.Text = "Low stock only";
            this.chkLowStockOnly.UseVisualStyleBackColor = true;
            //
            // btnAdd
            //
            this.btnAdd.Location = new System.Drawing.Point(470, 10);
            this.btnAdd.Size = new System.Drawing.Size(90, 27);
            this.btnAdd.Text = "+ Add Product";
            this.btnAdd.UseVisualStyleBackColor = true;
            //
            // btnEdit
            //
            this.btnEdit.Location = new System.Drawing.Point(566, 10);
            this.btnEdit.Size = new System.Drawing.Size(75, 27);
            this.btnEdit.Text = "Edit";
            //
            // btnDelete
            //
            this.btnDelete.Location = new System.Drawing.Point(647, 10);
            this.btnDelete.Size = new System.Drawing.Size(75, 27);
            this.btnDelete.Text = "Delete";
            //
            // btnStockIn
            //
            this.btnStockIn.Location = new System.Drawing.Point(728, 10);
            this.btnStockIn.Size = new System.Drawing.Size(85, 27);
            this.btnStockIn.Text = "Stock In";
            //
            // btnStockOut
            //
            this.btnStockOut.Location = new System.Drawing.Point(819, 10);
            this.btnStockOut.Size = new System.Drawing.Size(85, 27);
            this.btnStockOut.Text = "Stock Out";
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
            // tabTransactions
            //
            this.tabTransactions.Controls.Add(this.dgvTransactions);
            this.tabTransactions.Text = "Transaction History";
            //
            // dgvTransactions
            //
            this.dgvTransactions.AllowUserToAddRows = false;
            this.dgvTransactions.AllowUserToDeleteRows = false;
            this.dgvTransactions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTransactions.BackgroundColor = System.Drawing.Color.White;
            this.dgvTransactions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTransactions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTransactions.MultiSelect = false;
            this.dgvTransactions.ReadOnly = true;
            this.dgvTransactions.RowHeadersVisible = false;
            this.dgvTransactions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            //
            // menuStrip
            //
            this.menuProducts.Text = "Products";
            this.menuAddProduct.Text = "Add Product...";
            this.menuAddProduct.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuEditProduct.Text = "Edit Product...";
            this.menuDeleteProduct.Text = "Delete Product";
            this.menuStockIn.Text = "Stock In...";
            this.menuStockOut.Text = "Stock Out...";
            this.menuProducts.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuAddProduct,
                this.menuEditProduct,
                this.menuDeleteProduct,
                this.menuSep1,
                this.menuStockIn,
                this.menuStockOut });
            this.menuStrip.Items.Add(this.menuProducts);
            this.menuStrip.Text = "menuStrip";
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
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stock Management";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabInventory.ResumeLayout(false);
            this.tlpInventory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).EndInit();
            this.panelInventoryToolbar.ResumeLayout(false);
            this.panelInventoryToolbar.PerformLayout();
            this.tabTransactions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransactions)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
