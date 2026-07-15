using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Controls;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public sealed class ChalanForm : Form
    {
        private static readonly Font SummaryFont = new Font("Segoe UI", 12F);
        private static readonly Font GridFont = new Font("Segoe UI", 12F);
        private static readonly Font GridHeaderFont = new Font("Segoe UI", 12F, FontStyle.Bold);

        private readonly Invoice _invoice;
        private readonly DataGridView _grid;

        public ChalanForm(Invoice invoice)
        {
            _invoice = invoice ?? throw new ArgumentNullException(nameof(invoice));

            Text = $"Chalan - {_invoice.InvoiceNumber}";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = false;
            ClientSize = new Size(980, 620);
            MinimumSize = new Size(820, 480);
            UiStyles.Apply(this);

            var lblSummary = new Label
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
            summaryPanel.Controls.Add(lblSummary);

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
                    SelectionBackColor = Color.White,
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
                EnableHeadersVisualStyles = false
            };
            _grid.SelectionChanged += (s, e) => _grid.ClearSelection();
            ConfigureGrid();
            LoadItems();

            var btnClose = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Font = SummaryFont,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Size = new Size(90, 32),
                Location = new Point(874, 10)
            };
            btnClose.Click += (s, e) => Close();

            var footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 52,
                Padding = new Padding(16, 10, 16, 10)
            };
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

        private string BuildSummaryText()
        {
            var items = _invoice.Items ?? Enumerable.Empty<InvoiceLineItem>();
            var totalQty = items.Sum(i => i.Quantity);
            var itemCount = items.Count();
            var status = _invoice.IsActive ? "Active" : "Voided";

            return
                $"Invoice: {_invoice.InvoiceNumber}    Date: {_invoice.CreatedAt:g}    Status: {status}\r\n" +
                $"Customer: {(_invoice.CustomerName ?? "-")}\r\n" +
                $"Items: {itemCount}    Total quantity: {totalQty}";
        }

        private string GetExportBaseName()
        {
            var number = string.IsNullOrWhiteSpace(_invoice.InvoiceNumber)
                ? "Invoice"
                : _invoice.InvoiceNumber.Trim();

            foreach (var invalid in System.IO.Path.GetInvalidFileNameChars())
                number = number.Replace(invalid, '_');

            return $"Chalan - {number}";
        }

        private void ConfigureGrid()
        {
            _grid.AutoGenerateColumns = false;
            _grid.Columns.Clear();
            _grid.Columns.Add("Sku", "SKU");
            _grid.Columns.Add("Name", "Product");
            _grid.Columns.Add("Category", "Category");
            _grid.Columns.Add("Quantity", "Qty");

            _grid.Columns["Sku"].FillWeight = 90;
            _grid.Columns["Name"].FillWeight = 180;
            _grid.Columns["Category"].FillWeight = 110;
            _grid.Columns["Quantity"].FillWeight = 60;
            _grid.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _grid.Columns["Quantity"].DefaultCellStyle.Font = GridHeaderFont;

            foreach (DataGridViewColumn column in _grid.Columns)
                column.ReadOnly = true;
        }

        private void LoadItems()
        {
            _grid.Rows.Clear();
            foreach (var item in _invoice.Items ?? Enumerable.Empty<InvoiceLineItem>())
            {
                _grid.Rows.Add(
                    item.ProductSku ?? string.Empty,
                    item.ProductName ?? string.Empty,
                    item.ProductCategory ?? string.Empty,
                    item.Quantity);
            }

            _grid.ClearSelection();
        }
    }
}
