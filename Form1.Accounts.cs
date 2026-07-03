using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Controls;
using Stock_Managemnet.Models;

namespace Stock_Managemnet
{
    public partial class Form1
    {
        private Panel panelAccounts;
        private Panel panelAccountsNav;
        private ListBox lstAccountsNav;
        private Panel panelAccountsContent;
        private Panel panelAccountsChart;
        private Panel panelAccountsCustomerDue;
        private Panel panelAccountsCashLedger;
        private Panel panelAccountsSalesProfit;
        private Panel panelAccountsExpenses;
        private Label lblAccountsSummary;
        private DataGridView dgvAccountsChart;
        private TextBox txtAccountsSearch;
        private Button btnAccountsSearch;
        private Button btnAccountsReset;
        private Button btnReceivePayment;
        private DataGridView dgvCustomerDue;
        private ComboBox cmbCashLedgerAccount;
        private DateTimePicker dtpCashLedgerFrom;
        private DateTimePicker dtpCashLedgerTo;
        private CheckBox chkCashLedgerDateRange;
        private CustomerSelectControl cashLedgerCustomerSelect;
        private ComboBox cmbCashLedgerExpense;
        private Button btnCashLedgerSearch;
        private Button btnCashLedgerReset;
        private DataGridView dgvCashLedger;
        private Label lblCashBalance;
        private Label lblSalesProfitSummary;
        private TextBox txtProfitSearch;
        private Button btnProfitSearch;
        private Button btnProfitReset;
        private DateTimePicker dtpProfitFrom;
        private DateTimePicker dtpProfitTo;
        private CheckBox chkProfitDateRange;
        private DataGridView dgvSalesProfit;
        private Label lblExpensesSummary;
        private ComboBox cmbExpenseCategory;
        private TextBox txtExpenseSearch;
        private Button btnExpenseSearch;
        private Button btnExpenseReset;
        private DateTimePicker dtpExpenseFrom;
        private DateTimePicker dtpExpenseTo;
        private CheckBox chkExpenseDateRange;
        private Button btnRecordExpense;
        private DataGridView dgvExpenses;

        private const int AccountsControlHeight = 32;
        private const int AccountsToolbarGap = 10;
        private const int AccountsToolbarRowPadding = 6;
        private static readonly Color AccountsToolbarBack = Color.FromArgb(249, 250, 251);
        private static readonly Color AccountsToolbarBorder = Color.FromArgb(229, 231, 235);
        private static readonly Color AccountsLabelText = Color.FromArgb(75, 85, 99);

        private void InitializeAccountsUi()
        {
            panelAccounts = new Panel { Dock = DockStyle.Fill };
            panelAccountsNav = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(243, 244, 246),
                Padding = new Padding(12, 16, 12, 16)
            };
            lstAccountsNav = new ListBox
            {
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F),
                IntegralHeight = false,
                ItemHeight = 34
            };
            lstAccountsNav.Items.AddRange(new object[]
            {
                "Chart of Accounts",
                "Sales Profit",
                "Expenses",
                "Customer Due",
                "Cash Ledger"
            });

            panelAccountsContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };
            panelAccountsChart = CreateAccountsChartPanel();
            panelAccountsSalesProfit = CreateSalesProfitPanel();
            panelAccountsExpenses = CreateExpensesPanel();
            panelAccountsCustomerDue = CreateCustomerDuePanel();
            panelAccountsCashLedger = CreateCashLedgerPanel();

            panelAccountsContent.Controls.Add(panelAccountsCashLedger);
            panelAccountsContent.Controls.Add(panelAccountsCustomerDue);
            panelAccountsContent.Controls.Add(panelAccountsExpenses);
            panelAccountsContent.Controls.Add(panelAccountsSalesProfit);
            panelAccountsContent.Controls.Add(panelAccountsChart);

            panelAccountsNav.Controls.Add(lstAccountsNav);
            panelAccounts.Controls.Add(panelAccountsContent);
            panelAccounts.Controls.Add(panelAccountsNav);
            tabAccounts.Controls.Add(panelAccounts);

            ConfigureAccountsGrids();
            lstAccountsNav.SelectedIndex = 0;
            ShowAccountsSection(0);
        }

        private Panel CreateAccountsChartPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Visible = true };
            lblAccountsSummary = CreateAccountsSummaryLabel(Color.FromArgb(37, 99, 235));
            var summaryBar = WrapAccountsSummary(lblAccountsSummary);
            dgvAccountsChart = CreateAccountsGrid();
            panel.Controls.Add(dgvAccountsChart);
            panel.Controls.Add(summaryBar);
            return panel;
        }

        private Panel CreateSalesProfitPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Visible = false };

            var toolbarRow = CreateToolbarFlow();
            var lblSearch = CreateAccountsFieldLabel("Search:");
            txtProfitSearch = new TextBox { Width = 200, Height = AccountsControlHeight };
            btnProfitSearch = CreateAccountsButton("Search", 76);
            btnProfitReset = CreateAccountsButton("Reset", 76);
            chkProfitDateRange = CreateAccountsCheckBox("Date range");
            var lblFrom = CreateAccountsFieldLabel("From:");
            dtpProfitFrom = CreateAccountsDatePicker();
            var lblTo = CreateAccountsFieldLabel("To:");
            dtpProfitTo = CreateAccountsDatePicker();
            AddToolbarItem(toolbarRow, lblSearch);
            AddToolbarItem(toolbarRow, txtProfitSearch);
            AddToolbarItem(toolbarRow, btnProfitSearch);
            AddToolbarItem(toolbarRow, btnProfitReset);
            AddToolbarItem(toolbarRow, chkProfitDateRange, AccountsToolbarGap * 2);
            AddToolbarItem(toolbarRow, lblFrom);
            AddToolbarItem(toolbarRow, dtpProfitFrom);
            AddToolbarItem(toolbarRow, lblTo);
            AddToolbarItem(toolbarRow, dtpProfitTo, 0);

            var toolbar = CreateToolbarSection(toolbarRow);

            lblSalesProfitSummary = CreateAccountsSummaryLabel(Color.FromArgb(22, 101, 52));
            var summaryBar = WrapAccountsSummary(lblSalesProfitSummary);
            dgvSalesProfit = CreateAccountsGrid();

            panel.Controls.Add(dgvSalesProfit);
            panel.Controls.Add(summaryBar);
            panel.Controls.Add(toolbar);
            return panel;
        }

        private Panel CreateExpensesPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Visible = false };

            var filterRow = CreateToolbarFlow();
            btnRecordExpense = CreateAccountsPrimaryButton("+ Add Expense", 150);
            var lblCategory = CreateAccountsFieldLabel("Category:");
            cmbExpenseCategory = CreateAccountsComboBox(190);
            var lblSearch = CreateAccountsFieldLabel("Search:");
            txtExpenseSearch = new TextBox { Width = 160, Height = AccountsControlHeight };
            btnExpenseSearch = CreateAccountsButton("Search", 76);
            btnExpenseReset = CreateAccountsButton("Reset", 76);
            AddToolbarItem(filterRow, btnRecordExpense);
            AddToolbarItem(filterRow, lblCategory, AccountsToolbarGap * 2);
            AddToolbarItem(filterRow, cmbExpenseCategory);
            AddToolbarItem(filterRow, lblSearch, AccountsToolbarGap * 2);
            AddToolbarItem(filterRow, txtExpenseSearch);
            AddToolbarItem(filterRow, btnExpenseSearch);
            AddToolbarItem(filterRow, btnExpenseReset, 0);

            var dateRow = CreateToolbarFlow();
            chkExpenseDateRange = CreateAccountsCheckBox("Date range");
            var lblFrom = CreateAccountsFieldLabel("From:");
            dtpExpenseFrom = CreateAccountsDatePicker();
            var lblTo = CreateAccountsFieldLabel("To:");
            dtpExpenseTo = CreateAccountsDatePicker();
            AddToolbarItem(dateRow, chkExpenseDateRange);
            AddToolbarItem(dateRow, lblFrom);
            AddToolbarItem(dateRow, dtpExpenseFrom);
            AddToolbarItem(dateRow, lblTo);
            AddToolbarItem(dateRow, dtpExpenseTo, 0);

            var actionBar = CreateToolbarSection(filterRow, dateRow);

            lblExpensesSummary = CreateAccountsSummaryLabel(Color.FromArgb(153, 27, 27));
            var summaryBar = WrapAccountsSummary(lblExpensesSummary);
            dgvExpenses = CreateAccountsGrid();

            panel.Controls.Add(dgvExpenses);
            panel.Controls.Add(summaryBar);
            panel.Controls.Add(actionBar);
            return panel;
        }

        private Panel CreateCustomerDuePanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Visible = false };

            var searchRow = CreateToolbarFlow();
            var lblSearch = CreateAccountsFieldLabel("Search:");
            txtAccountsSearch = new TextBox { Width = 220, Height = AccountsControlHeight };
            btnAccountsSearch = CreateAccountsButton("Search", 76);
            btnAccountsReset = CreateAccountsButton("Reset", 76);
            AddToolbarItem(searchRow, lblSearch);
            AddToolbarItem(searchRow, txtAccountsSearch);
            AddToolbarItem(searchRow, btnAccountsSearch);
            AddToolbarItem(searchRow, btnAccountsReset, 0);

            btnReceivePayment = CreateAccountsPrimaryButton("Receive Payment", 150);
            var toolbar = CreateToolbarSplitSection(searchRow, btnReceivePayment);

            dgvCustomerDue = CreateAccountsGrid();
            panel.Controls.Add(dgvCustomerDue);
            panel.Controls.Add(toolbar);
            return panel;
        }

        private Panel CreateCashLedgerPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Visible = false };

            var customerRow = CreateToolbarFlow();
            var lblCustomer = CreateAccountsFieldLabel("Customer:");
            cashLedgerCustomerSelect = new CustomerSelectControl { Width = 240 };
            var lblExpense = CreateAccountsFieldLabel("Expense:");
            cmbCashLedgerExpense = CreateAccountsComboBox(200);
            btnCashLedgerSearch = CreateAccountsButton("Search", 76);
            btnCashLedgerReset = CreateAccountsButton("Reset", 76);
            AddToolbarItem(customerRow, lblCustomer);
            AddToolbarItem(customerRow, cashLedgerCustomerSelect);
            AddToolbarItem(customerRow, lblExpense, AccountsToolbarGap * 2);
            AddToolbarItem(customerRow, cmbCashLedgerExpense);
            AddToolbarItem(customerRow, btnCashLedgerSearch, AccountsToolbarGap * 2);
            AddToolbarItem(customerRow, btnCashLedgerReset, 0);

            var accountRow = CreateToolbarFlow();
            var lblAccount = CreateAccountsFieldLabel("Account:");
            cmbCashLedgerAccount = CreateAccountsComboBox(150);
            chkCashLedgerDateRange = CreateAccountsCheckBox("Date range");
            var lblFrom = CreateAccountsFieldLabel("From:");
            dtpCashLedgerFrom = CreateAccountsDatePicker();
            var lblTo = CreateAccountsFieldLabel("To:");
            dtpCashLedgerTo = CreateAccountsDatePicker();
            AddToolbarItem(accountRow, lblAccount);
            AddToolbarItem(accountRow, cmbCashLedgerAccount);
            AddToolbarItem(accountRow, chkCashLedgerDateRange, AccountsToolbarGap * 2);
            AddToolbarItem(accountRow, lblFrom);
            AddToolbarItem(accountRow, dtpCashLedgerFrom);
            AddToolbarItem(accountRow, lblTo);
            AddToolbarItem(accountRow, dtpCashLedgerTo, 0);

            lblCashBalance = new Label
            {
                AutoSize = true,
                ForeColor = Color.FromArgb(37, 99, 235),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Text = string.Empty
            };

            var filterBar = CreateToolbarSection(customerRow);
            var accountBar = CreateToolbarSplitSection(accountRow, lblCashBalance);

            dgvCashLedger = CreateAccountsGrid();
            panel.Controls.Add(dgvCashLedger);
            panel.Controls.Add(accountBar);
            panel.Controls.Add(filterBar);
            return panel;
        }

        private DataGridView CreateAccountsGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Margin = new Padding(0, 4, 0, 0)
            };
        }

        private FlowLayoutPanel CreateToolbarFlow()
        {
            return new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = new Padding(14, AccountsToolbarRowPadding, 14, AccountsToolbarRowPadding)
            };
        }

        private Panel CreateToolbarSection(params FlowLayoutPanel[] rows)
        {
            var section = CreateToolbarContainer();
            for (var i = 0; i < rows.Length; i++)
            {
                PrepareToolbarRow(rows[i]);
                section.Controls.Add(rows[i]);
                if (i < rows.Length - 1)
                    section.Controls.Add(CreateToolbarRowDivider());
            }

            section.Layout += (s, e) => LayoutStackedToolbar(section);
            return section;
        }

        private Panel CreateToolbarSplitSection(FlowLayoutPanel left, Control rightControl)
        {
            var section = CreateToolbarContainer();
            PrepareToolbarRow(left);
            left.Padding = new Padding(14, AccountsToolbarRowPadding, 8, AccountsToolbarRowPadding);
            rightControl.Margin = new Padding(0, AccountsToolbarRowPadding, 14, AccountsToolbarRowPadding);
            rightControl.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            section.Controls.Add(left);
            section.Controls.Add(rightControl);
            section.Layout += (s, e) => LayoutSplitToolbar(section, left, rightControl);
            return section;
        }

        private static void PrepareToolbarRow(FlowLayoutPanel row)
        {
            row.AutoSize = true;
            row.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            row.WrapContents = true;
            row.Margin = Padding.Empty;
        }

        private static Panel CreateToolbarContainer()
        {
            var section = new Panel
            {
                Dock = DockStyle.Top,
                BackColor = AccountsToolbarBack,
                Margin = new Padding(0, 0, 0, 8),
                Padding = Padding.Empty
            };
            section.Paint += PaintAccountsToolbarBorder;
            section.Resize += (s, e) => section.PerformLayout();
            return section;
        }

        private static int GetToolbarSectionWidth(Panel section)
        {
            if (section.ClientSize.Width > 0)
                return section.ClientSize.Width;

            return section.Parent?.ClientSize.Width > 0
                ? section.Parent.ClientSize.Width - section.Parent.Padding.Horizontal
                : 200;
        }

        private static void LayoutStackedToolbar(Panel section)
        {
            var width = GetToolbarSectionWidth(section);
            var y = 0;

            foreach (Control control in section.Controls)
            {
                if (control is FlowLayoutPanel flow)
                {
                    flow.Width = width;
                    flow.Location = new Point(0, y);
                    var height = flow.GetPreferredSize(new Size(width, 0)).Height;
                    flow.Height = height;
                    y += height;
                }
                else if (control.Height == 1)
                {
                    control.Width = width - 28;
                    control.Location = new Point(14, y);
                    y += 1;
                }
            }

            if (section.Height != y)
                section.Height = y;
        }

        private static void LayoutSplitToolbar(Panel section, FlowLayoutPanel left, Control rightControl)
        {
            var width = GetToolbarSectionWidth(section);
            var rightWidth = rightControl.PreferredSize.Width + rightControl.Margin.Horizontal;
            var leftWidth = Math.Max(120, width - rightWidth);

            left.Width = leftWidth;
            left.Location = new Point(0, 0);
            var leftHeight = left.GetPreferredSize(new Size(leftWidth, 0)).Height;
            left.Height = leftHeight;

            rightControl.Location = new Point(width - rightWidth, rightControl.Margin.Top);
            var rowHeight = Math.Max(leftHeight, rightControl.Height + rightControl.Margin.Vertical);

            if (section.Height != rowHeight)
                section.Height = rowHeight;
        }

        private static Panel CreateToolbarRowDivider()
        {
            return new Panel
            {
                Height = 1,
                BackColor = AccountsToolbarBorder
            };
        }

        private static Panel WrapAccountsSummary(Label label)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(14, 6, 14, 8),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 8)
            };
            label.Dock = DockStyle.Top;
            label.Margin = Padding.Empty;
            panel.Controls.Add(label);
            return panel;
        }

        private static Label CreateAccountsSummaryLabel(Color color)
        {
            return new Label
            {
                AutoSize = true,
                ForeColor = color,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Text = string.Empty
            };
        }

        private static Label CreateAccountsFieldLabel(string text)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                ForeColor = AccountsLabelText,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
        }

        private CheckBox CreateAccountsCheckBox(string text)
        {
            return new CheckBox
            {
                AutoSize = true,
                Text = text,
                Font = new Font("Segoe UI", 10F)
            };
        }

        private ComboBox CreateAccountsComboBox(int width)
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = width,
                Height = AccountsControlHeight
            };
        }

        private DateTimePicker CreateAccountsDatePicker()
        {
            return new DateTimePicker
            {
                Enabled = false,
                Format = DateTimePickerFormat.Short,
                Width = 120,
                Height = AccountsControlHeight
            };
        }

        private Button CreateAccountsButton(string text, int width)
        {
            return new Button
            {
                Text = text,
                Width = width,
                Height = AccountsControlHeight,
                FlatStyle = FlatStyle.Standard,
                UseVisualStyleBackColor = true
            };
        }

        private Button CreateAccountsPrimaryButton(string text, int width)
        {
            var button = new Button
            {
                Text = text,
                Width = width,
                Height = AccountsControlHeight,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(22, 101, 52),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private static void AddToolbarItem(FlowLayoutPanel flow, Control control, int rightGap = -1, int topGap = 2)
        {
            if (rightGap < 0)
                rightGap = AccountsToolbarGap;

            control.Margin = new Padding(0, topGap, rightGap, topGap);
            flow.Controls.Add(control);
        }

        private static void PaintAccountsToolbarBorder(object sender, PaintEventArgs e)
        {
            if (!(sender is Control control))
                return;

            using (var pen = new Pen(AccountsToolbarBorder))
            {
                e.Graphics.DrawLine(pen, 0, control.Height - 1, control.Width, control.Height - 1);
            }
        }

        private void ConfigureAccountsGrids()
        {
            dgvAccountsChart.AutoGenerateColumns = false;
            dgvAccountsChart.Columns.Clear();
            dgvAccountsChart.Columns.Add("Code", "Code");
            dgvAccountsChart.Columns.Add("Name", "Account");
            dgvAccountsChart.Columns.Add("Type", "Type");
            dgvAccountsChart.Columns.Add("Balance", "Balance");
            dgvAccountsChart.Columns["Balance"].DefaultCellStyle.Format = "C2";
            dgvAccountsChart.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvCustomerDue.AutoGenerateColumns = false;
            dgvCustomerDue.Columns.Clear();
            dgvCustomerDue.Columns.Add("CustomerName", "Customer");
            dgvCustomerDue.Columns.Add("Phone", "Phone");
            dgvCustomerDue.Columns.Add("TotalInvoiced", "Invoiced");
            dgvCustomerDue.Columns.Add("TotalPaid", "Paid");
            dgvCustomerDue.Columns.Add("BalanceDue", "Due");
            dgvCustomerDue.Columns.Add("OpenInvoiceCount", "Open Inv.");
            dgvCustomerDue.Columns["TotalInvoiced"].DefaultCellStyle.Format = "C2";
            dgvCustomerDue.Columns["TotalPaid"].DefaultCellStyle.Format = "C2";
            dgvCustomerDue.Columns["BalanceDue"].DefaultCellStyle.Format = "C2";
            dgvCustomerDue.Columns["TotalInvoiced"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCustomerDue.Columns["TotalPaid"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCustomerDue.Columns["BalanceDue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvCashLedger.AutoGenerateColumns = false;
            dgvCashLedger.Columns.Clear();
            dgvCashLedger.Columns.Add("Date", "Date");
            dgvCashLedger.Columns.Add("AccountName", "Account");
            dgvCashLedger.Columns.Add("Description", "Description");
            dgvCashLedger.Columns.Add("Reference", "Reference");
            dgvCashLedger.Columns.Add("Debit", "In");
            dgvCashLedger.Columns.Add("Credit", "Out");
            dgvCashLedger.Columns.Add("RunningBalance", "Balance");
            dgvCashLedger.Columns["Date"].DefaultCellStyle.Format = "g";
            dgvCashLedger.Columns["Debit"].DefaultCellStyle.Format = "C2";
            dgvCashLedger.Columns["Credit"].DefaultCellStyle.Format = "C2";
            dgvCashLedger.Columns["RunningBalance"].DefaultCellStyle.Format = "C2";
            dgvCashLedger.Columns["Debit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCashLedger.Columns["Credit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCashLedger.Columns["RunningBalance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvSalesProfit.AutoGenerateColumns = false;
            dgvSalesProfit.Columns.Clear();
            dgvSalesProfit.Columns.Add("Date", "Date");
            dgvSalesProfit.Columns.Add("InvoiceNumber", "Invoice #");
            dgvSalesProfit.Columns.Add("CustomerName", "Customer");
            dgvSalesProfit.Columns.Add("ProductName", "Product");
            dgvSalesProfit.Columns.Add("Quantity", "Qty");
            dgvSalesProfit.Columns.Add("SaleAmount", "Sale");
            dgvSalesProfit.Columns.Add("CostAmount", "Cost");
            dgvSalesProfit.Columns.Add("Profit", "Profit");
            dgvSalesProfit.Columns.Add("MarginPercent", "Margin %");
            dgvSalesProfit.Columns["Date"].DefaultCellStyle.Format = "g";
            dgvSalesProfit.Columns["SaleAmount"].DefaultCellStyle.Format = "C2";
            dgvSalesProfit.Columns["CostAmount"].DefaultCellStyle.Format = "C2";
            dgvSalesProfit.Columns["Profit"].DefaultCellStyle.Format = "C2";
            dgvSalesProfit.Columns["MarginPercent"].DefaultCellStyle.Format = "0.0'%'";
            dgvSalesProfit.Columns["SaleAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSalesProfit.Columns["CostAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSalesProfit.Columns["Profit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSalesProfit.Columns["MarginPercent"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvExpenses.AutoGenerateColumns = false;
            dgvExpenses.Columns.Clear();
            dgvExpenses.Columns.Add("Date", "Date");
            dgvExpenses.Columns.Add("Category", "Category");
            dgvExpenses.Columns.Add("PaidFrom", "Paid From");
            dgvExpenses.Columns.Add("Amount", "Amount");
            dgvExpenses.Columns.Add("Reference", "Reference");
            dgvExpenses.Columns.Add("Notes", "Notes");
            dgvExpenses.Columns["Date"].DefaultCellStyle.Format = "g";
            dgvExpenses.Columns["Amount"].DefaultCellStyle.Format = "C2";
            dgvExpenses.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void WireAccountsEvents()
        {
            lstAccountsNav.SelectedIndexChanged += (s, e) =>
            {
                if (lstAccountsNav.SelectedIndex >= 0)
                    ShowAccountsSection(lstAccountsNav.SelectedIndex);
            };
            btnAccountsSearch.Click += (s, e) => RefreshCustomerDue();
            btnAccountsReset.Click += (s, e) =>
            {
                txtAccountsSearch.Clear();
                RefreshCustomerDue();
            };
            btnReceivePayment.Click += BtnReceivePayment_Click;
            txtAccountsSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RefreshCustomerDue();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
            cmbCashLedgerAccount.SelectedIndexChanged += (s, e) => RefreshCashLedger();
            cashLedgerCustomerSelect.BindSearch(
                term => _repository.SearchCustomers(term),
                (customer, term) => _repository.CustomerMatchesSearchTerm(customer, term));
            cashLedgerCustomerSelect.SelectedCustomerChanged += (s, e) => RefreshCashLedger();
            chkCashLedgerDateRange.CheckedChanged += (s, e) =>
            {
                dtpCashLedgerFrom.Enabled = chkCashLedgerDateRange.Checked;
                dtpCashLedgerTo.Enabled = chkCashLedgerDateRange.Checked;
                RefreshCashLedger();
            };
            dtpCashLedgerFrom.ValueChanged += (s, e) => { if (chkCashLedgerDateRange.Checked) RefreshCashLedger(); };
            dtpCashLedgerTo.ValueChanged += (s, e) => { if (chkCashLedgerDateRange.Checked) RefreshCashLedger(); };
            btnCashLedgerSearch.Click += (s, e) => RefreshCashLedger();
            btnCashLedgerReset.Click += (s, e) => ResetCashLedgerFilters();
            cmbCashLedgerExpense.SelectedIndexChanged += (s, e) => RefreshCashLedger();
            dgvCustomerDue.CellDoubleClick += (s, e) => BtnReceivePayment_Click(s, e);
            btnProfitSearch.Click += (s, e) => RefreshSalesProfit();
            btnProfitReset.Click += (s, e) => ResetSalesProfitFilters();
            txtProfitSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RefreshSalesProfit();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
            chkProfitDateRange.CheckedChanged += (s, e) =>
            {
                dtpProfitFrom.Enabled = chkProfitDateRange.Checked;
                dtpProfitTo.Enabled = chkProfitDateRange.Checked;
                RefreshSalesProfit();
            };
            dtpProfitFrom.ValueChanged += (s, e) => { if (chkProfitDateRange.Checked) RefreshSalesProfit(); };
            dtpProfitTo.ValueChanged += (s, e) => { if (chkProfitDateRange.Checked) RefreshSalesProfit(); };
            btnRecordExpense.Click += BtnRecordExpense_Click;
            btnExpenseSearch.Click += (s, e) => RefreshExpenses();
            btnExpenseReset.Click += (s, e) => ResetExpenseFilters();
            txtExpenseSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RefreshExpenses();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
            cmbExpenseCategory.SelectedIndexChanged += (s, e) => RefreshExpenses();
            chkExpenseDateRange.CheckedChanged += (s, e) =>
            {
                dtpExpenseFrom.Enabled = chkExpenseDateRange.Checked;
                dtpExpenseTo.Enabled = chkExpenseDateRange.Checked;
                RefreshExpenses();
            };
            dtpExpenseFrom.ValueChanged += (s, e) => { if (chkExpenseDateRange.Checked) RefreshExpenses(); };
            dtpExpenseTo.ValueChanged += (s, e) => { if (chkExpenseDateRange.Checked) RefreshExpenses(); };
        }

        private void ShowAccountsSection(int index)
        {
            panelAccountsChart.Visible = index == 0;
            panelAccountsSalesProfit.Visible = index == 1;
            panelAccountsExpenses.Visible = index == 2;
            panelAccountsCustomerDue.Visible = index == 3;
            panelAccountsCashLedger.Visible = index == 4;

            if (index == 0) RefreshAccountsChart();
            else if (index == 1) RefreshSalesProfit();
            else if (index == 2) RefreshExpenses();
            else if (index == 3) RefreshCustomerDue();
            else RefreshCashLedger();

            RelayoutAccountsToolbars(GetVisibleAccountsPanel(index));
        }

        private Panel GetVisibleAccountsPanel(int index)
        {
            switch (index)
            {
                case 0: return panelAccountsChart;
                case 1: return panelAccountsSalesProfit;
                case 2: return panelAccountsExpenses;
                case 3: return panelAccountsCustomerDue;
                default: return panelAccountsCashLedger;
            }
        }

        private static void RelayoutAccountsToolbars(Control root)
        {
            if (root == null)
                return;

            foreach (Control child in root.Controls)
            {
                if (child is Panel panel && panel.BackColor == AccountsToolbarBack)
                    panel.PerformLayout();
            }
        }

        private void RefreshAccountsTab()
        {
            if (lstAccountsNav.SelectedIndex < 0)
                lstAccountsNav.SelectedIndex = 0;
            else
                ShowAccountsSection(lstAccountsNav.SelectedIndex);
        }

        private void RefreshAccountsChart()
        {
            dgvAccountsChart.Rows.Clear();
            foreach (var account in _repository.GetAccounts())
            {
                var balance = _repository.GetAccountBalance(account.Id);
                var idx = dgvAccountsChart.Rows.Add(
                    account.Code,
                    account.Name,
                    AccountTypeLabels.ToLabel(account.Type),
                    balance);
                dgvAccountsChart.Rows[idx].Tag = account;
            }

            var outstanding = _repository.GetTotalOutstanding();
            var cash = _repository.GetAccountBalance(SystemAccounts.CashId);
            var bank = _repository.GetAccountBalance(SystemAccounts.BankId);
            var profit = _repository.GetSalesProfitSummary();
            var expenses = _repository.GetTotalExpenses();
            lblAccountsSummary.Text =
                $"Customer due: {outstanding:C2}  |  Cash: {cash:C2}  |  Bank: {bank:C2}  |  Gross profit: {profit.GrossProfit:C2}  |  Expenses: {expenses:C2}";
        }

        private void RefreshSalesProfit()
        {
            if (dtpProfitTo.Value == default(DateTime))
            {
                dtpProfitTo.Value = DateTime.Today;
                dtpProfitFrom.Value = DateTime.Today.AddMonths(-1);
            }

            DateTime? from = chkProfitDateRange.Checked ? dtpProfitFrom.Value.Date : (DateTime?)null;
            DateTime? to = chkProfitDateRange.Checked ? dtpProfitTo.Value.Date : (DateTime?)null;
            var summary = _repository.GetSalesProfitSummary(from, to, txtProfitSearch.Text);
            lblSalesProfitSummary.Text =
                $"Sales: {summary.TotalSales:C2}  |  Cost: {summary.TotalCost:C2}  |  Gross profit: {summary.GrossProfit:C2}  |  Margin: {summary.MarginPercent:0.0}%";

            dgvSalesProfit.Rows.Clear();
            foreach (var row in _repository.GetSalesProfitLines(from, to, txtProfitSearch.Text))
            {
                var idx = dgvSalesProfit.Rows.Add(
                    row.Date,
                    row.InvoiceNumber,
                    row.CustomerName,
                    row.ProductName,
                    row.Quantity,
                    row.SaleAmount,
                    row.CostAmount,
                    row.Profit,
                    row.MarginPercent);

                if (row.Profit < 0)
                {
                    dgvSalesProfit.Rows[idx].Cells["Profit"].Style.ForeColor = Color.FromArgb(153, 27, 27);
                }
                else if (row.Profit > 0)
                {
                    dgvSalesProfit.Rows[idx].Cells["Profit"].Style.ForeColor = Color.FromArgb(22, 101, 52);
                }
            }
        }

        private void ResetSalesProfitFilters()
        {
            txtProfitSearch.Clear();
            chkProfitDateRange.Checked = false;
            dtpProfitTo.Value = DateTime.Today;
            dtpProfitFrom.Value = DateTime.Today.AddMonths(-1);
            dtpProfitFrom.Enabled = false;
            dtpProfitTo.Enabled = false;
            RefreshSalesProfit();
        }

        private void RefreshExpenses()
        {
            if (cmbExpenseCategory.Items.Count == 0)
            {
                var categories = _repository.GetExpenseAccounts().ToList();
                var items = new System.Collections.Generic.List<object>
                {
                    new { Id = Guid.Empty, Name = "All categories" }
                };
                items.AddRange(categories.Select(a => new { a.Id, a.Name }));
                cmbExpenseCategory.DisplayMember = "Name";
                cmbExpenseCategory.ValueMember = "Id";
                cmbExpenseCategory.DataSource = items;
                dtpExpenseTo.Value = DateTime.Today;
                dtpExpenseFrom.Value = DateTime.Today.AddMonths(-1);
            }

            DateTime? from = chkExpenseDateRange.Checked ? dtpExpenseFrom.Value.Date : (DateTime?)null;
            DateTime? to = chkExpenseDateRange.Checked ? dtpExpenseTo.Value.Date : (DateTime?)null;
            Guid? categoryId = null;
            if (cmbExpenseCategory.SelectedValue is Guid selectedId && selectedId != Guid.Empty)
                categoryId = selectedId;

            var total = _repository.GetTotalExpenses(from, to);
            lblExpensesSummary.Text = $"Total expenses: {total:C2}";

            dgvExpenses.Rows.Clear();
            foreach (var row in _repository.GetBusinessExpenses(from, to, txtExpenseSearch.Text, categoryId))
            {
                dgvExpenses.Rows.Add(
                    row.PaidAt,
                    row.ExpenseAccountName,
                    row.CashAccountName,
                    row.Amount,
                    row.Reference,
                    row.Notes);
            }
        }

        private void ResetExpenseFilters()
        {
            txtExpenseSearch.Clear();
            chkExpenseDateRange.Checked = false;
            dtpExpenseTo.Value = DateTime.Today;
            dtpExpenseFrom.Value = DateTime.Today.AddMonths(-1);
            dtpExpenseFrom.Enabled = false;
            dtpExpenseTo.Enabled = false;
            if (cmbExpenseCategory.Items.Count > 0)
                cmbExpenseCategory.SelectedIndex = 0;
            RefreshExpenses();
        }

        private void RefreshCustomerDue()
        {
            dgvCustomerDue.Rows.Clear();
            foreach (var row in _repository.GetCustomerDueReport(txtAccountsSearch.Text))
            {
                var idx = dgvCustomerDue.Rows.Add(
                    row.CustomerName,
                    row.Phone,
                    row.TotalInvoiced,
                    row.TotalPaid,
                    row.BalanceDue,
                    row.OpenInvoiceCount);
                dgvCustomerDue.Rows[idx].Tag = row;

                if (row.BalanceDue > 0)
                {
                    dgvCustomerDue.Rows[idx].Cells["BalanceDue"].Style.ForeColor = Color.FromArgb(153, 27, 27);
                    dgvCustomerDue.Rows[idx].Cells["BalanceDue"].Style.Font =
                        new Font(dgvCustomerDue.Font, FontStyle.Bold);
                }
            }
        }

        private void RefreshCashLedger()
        {
            if (cmbCashLedgerAccount.Items.Count == 0)
            {
                var accounts = _repository.GetCashAndBankAccounts().ToList();
                cmbCashLedgerAccount.DisplayMember = "Name";
                cmbCashLedgerAccount.ValueMember = "Id";
                cmbCashLedgerAccount.DataSource = accounts;
                dtpCashLedgerTo.Value = DateTime.Today;
                dtpCashLedgerFrom.Value = DateTime.Today.AddMonths(-1);
            }

            if (cmbCashLedgerExpense.Items.Count == 0)
            {
                var expenseItems = new System.Collections.Generic.List<object>
                {
                    new { Id = Guid.Empty, Name = "All expense types" }
                };
                expenseItems.AddRange(_repository.GetExpenseAccounts().Select(a => new { a.Id, a.Name }));
                cmbCashLedgerExpense.DisplayMember = "Name";
                cmbCashLedgerExpense.ValueMember = "Id";
                cmbCashLedgerExpense.DataSource = expenseItems;
            }

            dgvCashLedger.Rows.Clear();
            Guid? accountId = cmbCashLedgerAccount.SelectedValue as Guid?;
            DateTime? from = chkCashLedgerDateRange.Checked ? dtpCashLedgerFrom.Value.Date : (DateTime?)null;
            DateTime? to = chkCashLedgerDateRange.Checked ? dtpCashLedgerTo.Value.Date : (DateTime?)null;
            Guid? expenseAccountId = null;
            if (cmbCashLedgerExpense.SelectedValue is Guid selectedExpenseId && selectedExpenseId != Guid.Empty)
                expenseAccountId = selectedExpenseId;

            Guid? customerId = cashLedgerCustomerSelect.SelectedCustomer?.Id;

            foreach (var row in _repository.GetCashLedger(
                accountId,
                from,
                to,
                customerId,
                expenseAccountId))
            {
                dgvCashLedger.Rows.Add(
                    row.Date,
                    row.AccountName,
                    row.Description,
                    row.Reference,
                    row.Debit > 0 ? row.Debit : (object)DBNull.Value,
                    row.Credit > 0 ? row.Credit : (object)DBNull.Value,
                    row.RunningBalance);
            }

            if (accountId.HasValue)
                lblCashBalance.Text = $"Balance: {_repository.GetAccountBalance(accountId.Value):C2}";
            else
                lblCashBalance.Text = string.Empty;
        }

        private void ResetCashLedgerFilters()
        {
            cashLedgerCustomerSelect.ClearSelection();
            if (cmbCashLedgerExpense.Items.Count > 0)
                cmbCashLedgerExpense.SelectedIndex = 0;
            chkCashLedgerDateRange.Checked = false;
            dtpCashLedgerTo.Value = DateTime.Today;
            dtpCashLedgerFrom.Value = DateTime.Today.AddMonths(-1);
            dtpCashLedgerFrom.Enabled = false;
            dtpCashLedgerTo.Enabled = false;
            RefreshCashLedger();
        }

        private CustomerDueRow GetSelectedCustomerDueRow()
        {
            if (dgvCustomerDue.SelectedRows.Count == 0) return null;
            return dgvCustomerDue.SelectedRows[0].Tag as CustomerDueRow;
        }

        private void BtnReceivePayment_Click(object sender, EventArgs e)
        {
            Customer customer = null;
            var dueRow = GetSelectedCustomerDueRow();
            if (dueRow != null)
                customer = _repository.GetCustomer(dueRow.CustomerId);

            using (var form = new ReceivePaymentForm(_repository, customer))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    RefreshAll();
            }
        }

        private void BtnRecordExpense_Click(object sender, EventArgs e)
        {
            using (var form = new RecordExpenseForm(_repository))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    RefreshAll();
            }
        }
    }
}
