using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class MultiStockInForm : Form
    {
        private sealed class CartLine
        {
            public Product Product { get; set; }
            public Panel RowPanel { get; set; }
            public NumericUpDown Quantity { get; set; }
        }

        private readonly StockRepository _repository;
        private readonly List<CartLine> _cartLines = new List<CartLine>();

        public MultiStockInForm(StockRepository repository, IList<Product> products)
        {
            _repository = repository;
            InitializeComponent();
            UiStyles.Apply(this);
            Text = "Stock In";
            AcceptButton = null;

            panelCart.Resize += (s, e) => UpdateCartRowWidths();
            foreach (var product in products?.Where(p => p != null) ?? Enumerable.Empty<Product>())
                AddCartLine(product);

            supplierSelect.BindSearch(
                term => _repository.SearchSuppliers(term),
                (supplier, term) => _repository.SupplierMatchesSearchTerm(supplier, term));
        }

        private void AddCartLine(Product product)
        {
            var row = new Panel
            {
                Height = 44,
                Margin = new Padding(8, 4, 8, 4)
            };

            var btnRemove = new Button
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(row.Width - 83, 8),
                Size = new Size(75, 27),
                Text = "Remove",
                TabStop = false
            };
            btnRemove.Click += (s, e) => RemoveCartLine(row);

            var numQty = new NumericUpDown
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(row.Width - 168, 9),
                Width = 80,
                Minimum = 1,
                Maximum = 999999,
                Value = 1,
                TextAlign = HorizontalAlignment.Center
            };

            var lblCurrent = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                AutoSize = true,
                Location = new Point(row.Width - 268, 12),
                Text = $"In stock: {product.Quantity}",
                TextAlign = ContentAlignment.MiddleRight
            };

            var lblProduct = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AutoSize = false,
                Location = new Point(8, 12),
                Size = new Size(row.Width - 280, 20),
                Text = $"{product.Sku} — {product.Name}",
                TextAlign = ContentAlignment.MiddleLeft
            };

            row.Controls.Add(lblProduct);
            row.Controls.Add(lblCurrent);
            row.Controls.Add(numQty);
            row.Controls.Add(btnRemove);
            row.Resize += (s, e) => LayoutCartRow(row, lblProduct, lblCurrent, numQty, btnRemove);

            _cartLines.Add(new CartLine
            {
                Product = product,
                RowPanel = row,
                Quantity = numQty
            });

            flowCart.Controls.Add(row);
            UpdateCartRowWidths();
        }

        private static void LayoutCartRow(
            Panel row,
            Label lblProduct,
            Label lblCurrent,
            NumericUpDown numQty,
            Button btnRemove)
        {
            btnRemove.Left = row.Width - btnRemove.Width - 8;
            numQty.Left = btnRemove.Left - numQty.Width - 12;
            lblCurrent.Left = numQty.Left - lblCurrent.PreferredWidth - 16;
            lblProduct.Width = Math.Max(120, lblCurrent.Left - lblProduct.Left - 8);
        }

        private void UpdateCartRowWidths()
        {
            var width = Math.Max(200, panelCart.ClientSize.Width - 24);
            foreach (var line in _cartLines)
            {
                line.RowPanel.Width = width;
                var row = line.RowPanel;
                var lblProduct = row.Controls.OfType<Label>().First(l => !l.Text.StartsWith("In stock:"));
                var lblCurrent = row.Controls.OfType<Label>().First(l => l.Text.StartsWith("In stock:"));
                var numQty = row.Controls.OfType<NumericUpDown>().First();
                var btnRemove = row.Controls.OfType<Button>().First();
                LayoutCartRow(row, lblProduct, lblCurrent, numQty, btnRemove);
            }
        }

        private void RemoveCartLine(Panel row)
        {
            var line = _cartLines.FirstOrDefault(l => l.RowPanel == row);
            if (line == null)
                return;

            _cartLines.Remove(line);
            flowCart.Controls.Remove(row);
            row.Dispose();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (ActiveControl == txtNotes)
                    return base.ProcessDialogKey(keyData);

                if (supplierSelect.IsInputFocused || supplierSelect.IsDropDownOpen)
                {
                    supplierSelect.TrySelectHighlightedSupplier();
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            var lines = _cartLines
                .Where(line => line.Quantity.Value > 0)
                .ToList();

            if (lines.Count == 0)
            {
                MessageBox.Show("Add at least one product to stock in.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var supplierId = supplierSelect.SelectedSupplier?.Id;
            var notes = txtNotes.Text.Trim();
            var payload = lines
                .Select(line => (line.Product.Id, (int)line.Quantity.Value))
                .ToList();

            var error = _repository.StockInMultiple(payload, notes, supplierId);
            if (error != null)
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
