using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class MultiStockOutForm : Form
    {
        private sealed class CartLine
        {
            public Product Product { get; set; }
            public Panel RowPanel { get; set; }
            public NumericUpDown Quantity { get; set; }
        }

        private readonly StockRepository _repository;
        private readonly List<CartLine> _cartLines = new List<CartLine>();

        public MultiStockOutForm(StockRepository repository, IList<Product> products)
        {
            _repository = repository;
            InitializeComponent();
            UiStyles.Apply(this);
            Text = "Stock Out";
            AcceptButton = null;

            panelCart.Resize += (s, e) => UpdateCartRowWidths();
            foreach (var product in products?.Where(p => p != null) ?? Enumerable.Empty<Product>())
                AddCartLine(product);

            customerSelect.BindSearch(
                term => _repository.SearchCustomers(term),
                (customer, term) => _repository.CustomerMatchesSearchTerm(customer, term));
        }

        private void AddCartLine(Product product)
        {
            var maxQty = Math.Max(1, product.Quantity);
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
                Maximum = maxQty,
                Value = 1,
                TextAlign = HorizontalAlignment.Center
            };

            var lblAvailable = new Label
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
            row.Controls.Add(lblAvailable);
            row.Controls.Add(numQty);
            row.Controls.Add(btnRemove);
            row.Resize += (s, e) => LayoutCartRow(row, lblProduct, lblAvailable, numQty, btnRemove);

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
            Label lblAvailable,
            NumericUpDown numQty,
            Button btnRemove)
        {
            btnRemove.Left = row.Width - btnRemove.Width - 8;
            numQty.Left = btnRemove.Left - numQty.Width - 12;
            lblAvailable.Left = numQty.Left - lblAvailable.PreferredWidth - 16;
            lblProduct.Width = Math.Max(120, lblAvailable.Left - lblProduct.Left - 8);
        }

        private void UpdateCartRowWidths()
        {
            var width = Math.Max(200, panelCart.ClientSize.Width - 24);
            foreach (var line in _cartLines)
            {
                line.RowPanel.Width = width;
                var row = line.RowPanel;
                var lblProduct = row.Controls.OfType<Label>().First(l => !l.Text.StartsWith("In stock:"));
                var lblAvailable = row.Controls.OfType<Label>().First(l => l.Text.StartsWith("In stock:"));
                var numQty = row.Controls.OfType<NumericUpDown>().First();
                var btnRemove = row.Controls.OfType<Button>().First();
                LayoutCartRow(row, lblProduct, lblAvailable, numQty, btnRemove);
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

        private StockOutRequest BuildRequest()
        {
            var lines = _cartLines
                .Where(line => line.Quantity.Value > 0)
                .Select(line => new StockOutLineItem
                {
                    ProductId = line.Product.Id,
                    Quantity = (int)line.Quantity.Value
                })
                .ToList();

            return new StockOutRequest
            {
                CustomerId = customerSelect.SelectedCustomer?.Id,
                Notes = txtNotes.Text.Trim(),
                Lines = lines
            };
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (ActiveControl == txtNotes)
                    return base.ProcessDialogKey(keyData);

                if (customerSelect.IsInputFocused || customerSelect.IsDropDownOpen)
                {
                    customerSelect.TrySelectHighlightedCustomer();
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        private void BtnContinue_Click(object sender, EventArgs e)
        {
            var request = BuildRequest();
            if (request.Lines.Count == 0)
            {
                MessageBox.Show("Add at least one product to the cart.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var validationError = _repository.ValidateStockOut(request);
            if (validationError != null)
            {
                MessageBox.Show(validationError, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var form = new InvoiceForm(_repository, request))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
