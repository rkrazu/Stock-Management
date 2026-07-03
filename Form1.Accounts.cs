using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
            lblAccountsSummary = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                Height = 28,
                Padding = new Padding(0, 4, 0, 8),
                Text = "Accounts"
            };
            dgvAccountsChart = new DataGridView
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
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            panel.Controls.Add(dgvAccountsChart);
            panel.Controls.Add(lblAccountsSummary);
            return panel;
        }

        private Panel CreateSalesProfitPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Visible = false };
            var toolbar = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(0, 4, 0, 8) };

            txtProfitSearch = new TextBox { Location = new Point(0, 8), Width = 180 };
            btnProfitSearch = new Button { Location = new Point(186, 6), Width = 65, Height = 27, Text = "Search" };
            btnProfitReset = new Button { Location = new Point(257, 6), Width = 65, Height = 27, Text = "Reset" };
            chkProfitDateRange = new CheckBox { AutoSize = true, Location = new Point(340, 9), Text = "Date range" };
            dtpProfitFrom = new DateTimePicker
            {
                Enabled = false,
                Format = DateTimePickerFormat.Short,
                Location = new Point(450, 7),
                Width = 110
            };
            dtpProfitTo = new DateTimePicker
            {
                Enabled = false,
                Format = DateTimePickerFormat.Short,
                Location = new Point(570, 7),
                Width = 110
            };

            toolbar.Controls.Add(txtProfitSearch);
            toolbar.Controls.Add(btnProfitSearch);
            toolbar.Controls.Add(btnProfitReset);
            toolbar.Controls.Add(chkProfitDateRange);
            toolbar.Controls.Add(dtpProfitFrom);
            toolbar.Controls.Add(dtpProfitTo);

            lblSalesProfitSummary = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                Padding = new Padding(0, 4, 0, 8),
                ForeColor = Color.FromArgb(22, 101, 52)
            };

            dgvSalesProfit = new DataGridView
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
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            panel.Controls.Add(dgvSalesProfit);
            panel.Controls.Add(lblSalesProfitSummary);
            panel.Controls.Add(toolbar);
            return panel;
        }

        private Panel CreateExpensesPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Visible = false };

            var actionBar = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(0, 8, 0, 4) };
            btnRecordExpense = new Button
            {
                Dock = DockStyle.Left,
                Width = 160,
                Height = 32,
                Text = "+ Add Expense",
                BackColor = Color.FromArgb(22, 101, 52),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRecordExpense.FlatAppearance.BorderSize = 0;

            var lblExpenseHint = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Left,
                Padding = new Padding(12, 8, 0, 0),
                ForeColor = Color.FromArgb(107, 114, 128),
                Text = "Record rent, salary, snacks, transport, and other costs paid from cash or bank."
            };

            actionBar.Controls.Add(lblExpenseHint);
            actionBar.Controls.Add(btnRecordExpense);

            var toolbar = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(0, 4, 0, 8) };

            var lblCategory = new Label { AutoSize = true, Location = new Point(0, 10), Text = "Category:" };
            cmbExpenseCategory = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(68, 7),
                Width = 170
            };

            txtExpenseSearch = new TextBox { Location = new Point(250, 8), Width = 160 };
            btnExpenseSearch = new Button { Location = new Point(416, 6), Width = 70, Height = 27, Text = "Search" };
            btnExpenseReset = new Button { Location = new Point(492, 6), Width = 70, Height = 27, Text = "Reset" };
            chkExpenseDateRange = new CheckBox { AutoSize = true, Location = new Point(572, 9), Text = "Date range" };
            dtpExpenseFrom = new DateTimePicker
            {
                Enabled = false,
                Format = DateTimePickerFormat.Short,
                Location = new Point(670, 7),
                Width = 110
            };
            dtpExpenseTo = new DateTimePicker
            {
                Enabled = false,
                Format = DateTimePickerFormat.Short,
                Location = new Point(786, 7),
                Width = 110
            };

            toolbar.Controls.Add(lblCategory);
            toolbar.Controls.Add(cmbExpenseCategory);
            toolbar.Controls.Add(txtExpenseSearch);
            toolbar.Controls.Add(btnExpenseSearch);
            toolbar.Controls.Add(btnExpenseReset);
            toolbar.Controls.Add(chkExpenseDateRange);
            toolbar.Controls.Add(dtpExpenseFrom);
            toolbar.Controls.Add(dtpExpenseTo);

            lblExpensesSummary = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                Height = 28,
                Padding = new Padding(0, 4, 0, 8),
                ForeColor = Color.FromArgb(153, 27, 27)
            };

            dgvExpenses = new DataGridView
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
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            panel.Controls.Add(dgvExpenses);
            panel.Controls.Add(lblExpensesSummary);
            panel.Controls.Add(toolbar);
            panel.Controls.Add(actionBar);
            return panel;
        }

        private Panel CreateCustomerDuePanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Visible = false };
            var toolbar = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(0, 4, 0, 8) };

            txtAccountsSearch = new TextBox { Location = new Point(0, 8), Width = 200 };
            btnAccountsSearch = new Button { Location = new Point(206, 6), Width = 70, Height = 27, Text = "Search" };
            btnAccountsReset = new Button { Location = new Point(282, 6), Width = 70, Height = 27, Text = "Reset" };
            btnReceivePayment = new Button
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(700, 6),
                Width = 130,
                Height = 27,
                Text = "Receive Payment"
            };

            toolbar.Controls.Add(txtAccountsSearch);
            toolbar.Controls.Add(btnAccountsSearch);
            toolbar.Controls.Add(btnAccountsReset);
            toolbar.Controls.Add(btnReceivePayment);

            dgvCustomerDue = new DataGridView
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
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            panel.Controls.Add(dgvCustomerDue);
            panel.Controls.Add(toolbar);
            return panel;
        }

        private Panel CreateCashLedgerPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Visible = false };
            var toolbar = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(0, 4, 0, 8) };

            var lblAccount = new Label { AutoSize = true, Location = new Point(0, 10), Text = "Account:" };
            cmbCashLedgerAccount = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(62, 7),
                Width = 140
            };
            chkCashLedgerDateRange = new CheckBox { AutoSize = true, Location = new Point(220, 9), Text = "Date range" };
            dtpCashLedgerFrom = new DateTimePicker
            {
                Enabled = false,
                Format = DateTimePickerFormat.Short,
                Location = new Point(330, 7),
                Width = 110
            };
            dtpCashLedgerTo = new DateTimePicker
            {
                Enabled = false,
                Format = DateTimePickerFormat.Short,
                Location = new Point(450, 7),
                Width = 110
            };
            lblCashBalance = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                AutoSize = true,
                Location = new Point(700, 10),
                ForeColor = Color.FromArgb(37, 99, 235),
                Text = string.Empty
            };

            toolbar.Controls.Add(lblAccount);
            toolbar.Controls.Add(cmbCashLedgerAccount);
            toolbar.Controls.Add(chkCashLedgerDateRange);
            toolbar.Controls.Add(dtpCashLedgerFrom);
            toolbar.Controls.Add(dtpCashLedgerTo);
            toolbar.Controls.Add(lblCashBalance);

            dgvCashLedger = new DataGridView
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
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            panel.Controls.Add(dgvCashLedger);
            panel.Controls.Add(toolbar);
            return panel;
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
            chkCashLedgerDateRange.CheckedChanged += (s, e) =>
            {
                dtpCashLedgerFrom.Enabled = chkCashLedgerDateRange.Checked;
                dtpCashLedgerTo.Enabled = chkCashLedgerDateRange.Checked;
                RefreshCashLedger();
            };
            dtpCashLedgerFrom.ValueChanged += (s, e) => { if (chkCashLedgerDateRange.Checked) RefreshCashLedger(); };
            dtpCashLedgerTo.ValueChanged += (s, e) => { if (chkCashLedgerDateRange.Checked) RefreshCashLedger(); };
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

            dgvCashLedger.Rows.Clear();
            Guid? accountId = cmbCashLedgerAccount.SelectedValue as Guid?;
            DateTime? from = chkCashLedgerDateRange.Checked ? dtpCashLedgerFrom.Value.Date : (DateTime?)null;
            DateTime? to = chkCashLedgerDateRange.Checked ? dtpCashLedgerTo.Value.Date : (DateTime?)null;

            foreach (var row in _repository.GetCashLedger(accountId, from, to))
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
