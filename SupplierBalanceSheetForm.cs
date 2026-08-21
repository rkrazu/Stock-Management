using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Controls;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public sealed class SupplierBalanceSheetForm : Form
    {
        private static readonly Font SummaryFont = new Font("Segoe UI", 12F);
        private static readonly Font GridFont = new Font("Segoe UI", 12F);
        private static readonly Font GridHeaderFont = new Font("Segoe UI", 12F, FontStyle.Bold);

        private readonly StockRepository _repository;
        private SupplierDueRow _dueRow;
        private readonly Label _lblSummary;
        private readonly DataGridView _grid;
        private readonly Button _btnVoidPayment;
        private readonly Button _btnViewReceipt;

        public event EventHandler PaymentsChanged;

        public SupplierBalanceSheetForm(StockRepository repository, SupplierDueRow dueRow)
        {
            _repository = repository;
            _dueRow = dueRow ?? throw new ArgumentNullException(nameof(dueRow));

            Text = $"Supplier Balance Sheet - {dueRow.SupplierName}";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = false;
            ClientSize = new Size(1400, 640);
            MinimumSize = new Size(1200, 520);
            UiStyles.Apply(this);

            _lblSummary = new Label
            {
                Dock = DockStyle.Fill,
                Font = SummaryFont,
                Padding = new Padding(4, 8, 4, 4),
                Text = BuildSummaryText()
            };

            var summaryPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 92,
                Padding = new Padding(16, 12, 16, 8),
                BackColor = Color.FromArgb(249, 250, 251)
            };
            summaryPanel.Controls.Add(_lblSummary);

            _grid = new DataGridView
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
                Margin = new Padding(0, 4, 0, 0),
                Font = GridFont,
                DefaultCellStyle =
                {
                    Font = GridFont,
                    BackColor = Color.White,
                    ForeColor = SystemColors.ControlText,
                    SelectionBackColor = Color.FromArgb(219, 234, 254),
                    SelectionForeColor = SystemColors.ControlText
                },
                ColumnHeadersDefaultCellStyle =
                {
                    Font = GridHeaderFont,
                    BackColor = Color.FromArgb(243, 244, 246),
                    ForeColor = Color.FromArgb(31, 41, 55),
                    SelectionBackColor = Color.FromArgb(243, 244, 246),
                    SelectionForeColor = Color.FromArgb(31, 41, 55)
                },
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 36 },
                EnableHeadersVisualStyles = false,
                StandardTab = true
            };
            ConfigureGrid();
            LoadLedger();

            _btnVoidPayment = new Button
            {
                Text = "Void Payment",
                Font = SummaryFont,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Size = new Size(140, 32),
                Location = new Point(16, 10)
            };
            _btnVoidPayment.Click += BtnVoidPayment_Click;

            _btnViewReceipt = new Button
            {
                Text = "View Receipt",
                Font = SummaryFont,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Size = new Size(140, 32),
                Location = new Point(166, 10)
            };
            _btnViewReceipt.Click += BtnViewReceipt_Click;

            var btnClose = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Font = SummaryFont,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Size = new Size(90, 32),
                Location = new Point(1294, 10)
            };
            btnClose.Click += (s, e) => Close();

            var footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 52,
                Padding = new Padding(16, 10, 16, 10)
            };
            footer.Controls.Add(_btnVoidPayment);
            footer.Controls.Add(_btnViewReceipt);
            footer.Controls.Add(btnClose);

            var content = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 0, 16, 0)
            };
            content.Controls.Add(_grid);
            content.Controls.Add(summaryPanel);
            GridExportUi.Enable(_grid, GetExportBaseName());

            Controls.Add(content);
            Controls.Add(footer);
            AcceptButton = btnClose;
        }

        private string GetExportBaseName()
        {
            var name = string.IsNullOrWhiteSpace(_dueRow.SupplierName)
                ? "Supplier"
                : _dueRow.SupplierName.Trim();

            foreach (var invalid in System.IO.Path.GetInvalidFileNameChars())
                name = name.Replace(invalid, '_');

            return $"Supplier Balance Sheet - {name}";
        }

        private string BuildSummaryText()
        {
            return
                $"Supplier: {_dueRow.SupplierName}\r\n" +
                $"Phone: {(string.IsNullOrWhiteSpace(_dueRow.Phone) ? "-" : _dueRow.Phone)}\r\n" +
                $"Purchased: {_dueRow.TotalPurchased:C2}    Paid: {_dueRow.TotalPaid:C2}    " +
                (_dueRow.BalanceDue >= 0
                    ? $"Balance due: {_dueRow.BalanceDue:C2}"
                    : $"Advance / credit: {Math.Abs(_dueRow.BalanceDue):C2}") +
                $"    Purchases: {_dueRow.PurchaseCount}";
        }

        private void ConfigureGrid()
        {
            _grid.AutoGenerateColumns = false;
            _grid.Columns.Clear();
            _grid.Columns.Add("Date", "Date");
            _grid.Columns.Add("EntryType", "Type");
            _grid.Columns.Add("Description", "Description");
            _grid.Columns.Add("Reference", "Reference");
            _grid.Columns.Add("Debit", "Debit");
            _grid.Columns.Add("Credit", "Credit");
            _grid.Columns.Add("Balance", "Balance");

            _grid.Columns["Date"].DefaultCellStyle.Format = "g";
            _grid.Columns["Date"].FillWeight = 90;
            _grid.Columns["EntryType"].FillWeight = 55;
            _grid.Columns["Description"].FillWeight = 160;
            _grid.Columns["Reference"].FillWeight = 70;
            _grid.Columns["Debit"].DefaultCellStyle.Format = "C2";
            _grid.Columns["Credit"].DefaultCellStyle.Format = "C2";
            _grid.Columns["Balance"].DefaultCellStyle.Format = "C2";
            _grid.Columns["Debit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _grid.Columns["Credit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _grid.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _grid.Columns["Debit"].FillWeight = 70;
            _grid.Columns["Credit"].FillWeight = 70;
            _grid.Columns["Balance"].FillWeight = 75;

            foreach (DataGridViewColumn column in _grid.Columns)
                column.ReadOnly = true;
        }

        private void LoadLedger()
        {
            _grid.Rows.Clear();
            foreach (var row in _repository.GetSupplierLedger(_dueRow.SupplierId))
            {
                var idx = _grid.Rows.Add(
                    row.Date,
                    row.EntryType,
                    row.Description,
                    row.Reference,
                    row.Debit > 0 ? row.Debit : (object)null,
                    row.Credit > 0 ? row.Credit : (object)null,
                    row.Balance);
                _grid.Rows[idx].Tag = row;

                if (row.EntryType == "Debit")
                {
                    _grid.Rows[idx].Cells["Debit"].Style.ForeColor = Color.FromArgb(153, 27, 27);
                    _grid.Rows[idx].Cells["Debit"].Style.SelectionForeColor = Color.FromArgb(153, 27, 27);
                }
                else if (row.EntryType == "Credit")
                {
                    _grid.Rows[idx].Cells["Credit"].Style.ForeColor = Color.FromArgb(22, 101, 52);
                    _grid.Rows[idx].Cells["Credit"].Style.SelectionForeColor = Color.FromArgb(22, 101, 52);
                }
            }
        }

        private void RefreshDueRow()
        {
            _dueRow = _repository.GetSupplierDueReport()
                .FirstOrDefault(r => r.SupplierId == _dueRow.SupplierId)
                ?? _dueRow;
            _lblSummary.Text = BuildSummaryText();
            Text = $"Supplier Balance Sheet - {_dueRow.SupplierName}";
        }

        private void BtnVoidPayment_Click(object sender, EventArgs e)
        {
            if (!(_grid.CurrentRow?.Tag is SupplierLedgerRow row)
                || !row.PaymentId.HasValue
                || row.EntryType != "Credit")
            {
                MessageBox.Show(
                    "Select a payment (Credit) row to void.",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Void this payment of {row.Credit:C2}?\n\n" +
                "This restores the supplier due and returns the amount to the cash/bank balance.",
                "Void Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            var error = _repository.VoidSupplierPayment(row.PaymentId.Value, "Corrected wrong payment");
            if (error != null)
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RefreshDueRow();
            LoadLedger();
            PaymentsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void BtnViewReceipt_Click(object sender, EventArgs e)
        {
            if (!(_grid.CurrentRow?.Tag is SupplierLedgerRow row)
                || !row.PaymentId.HasValue
                || row.EntryType != "Credit")
            {
                MessageBox.Show(
                    "Select a payment (Credit) row that has a receipt photo.",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var payment = _repository.GetSupplierPayment(row.PaymentId.Value);
            if (payment == null || !payment.HasReceipt)
            {
                MessageBox.Show(
                    "No receipt photo is attached to this payment.",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var error = _repository.OpenPaymentReceipt(payment.ReceiptRelativePath);
            if (error != null)
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
