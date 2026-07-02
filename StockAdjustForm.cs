using System;
using System.Drawing;
using System.Windows.Forms;
using Stock_Managemnet.Controls;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class StockAdjustForm : Form
    {
        private readonly StockRepository _repository;
        private readonly Product _product;
        private readonly TransactionType _type;

        public StockAdjustForm(StockRepository repository, Product product, TransactionType type)
        {
            _repository = repository;
            _product = product;
            _type = type;
            InitializeComponent();
            UiStyles.Apply(this);

            btnApply.BringToFront();
            btnCancel.BringToFront();

            Text = type == TransactionType.StockIn ? "Stock In" : "Stock Out";
            lblProduct.Text = $"{product.Name} ({product.Sku})";
            lblAvailable.Text = type == TransactionType.StockOut
                ? $"Available: {product.Quantity}"
                : $"Current stock: {product.Quantity}";

            if (type == TransactionType.StockOut)
                ConfigureStockOutLayout();
        }

        private void ConfigureStockOutLayout()
        {
            panelCustomer.Visible = true;
            AcceptButton = null;

            customerSelect.BindSearch(
                term => _repository.SearchCustomers(term),
                (customer, term) => _repository.CustomerMatchesSearchTerm(customer, term));

            lblQuantity.Location = new Point(20, 145);
            numQuantity.Location = new Point(120, 142);
            lblNotes.Location = new Point(20, 180);
            AlignNotesWithCustomerSelect();
            txtNotes.Height = 70;

            ClientSize = new Size(580, 330);
            Shown += StockOutForm_Shown;
            Resize += StockOutForm_Resize;
        }

        private void StockOutForm_Resize(object sender, EventArgs e)
        {
            if (panelCustomer.Visible)
                AlignNotesWithCustomerSelect();
        }

        private void StockOutForm_Shown(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                ActiveControl = null;
                customerSelect.HideDropDownIfOpen();
            }));
        }

        private void AlignNotesWithCustomerSelect()
        {
            txtNotes.Location = new Point(customerSelect.Left, 200);
            txtNotes.Width = customerSelect.Width;
            txtNotes.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (_type == TransactionType.StockOut && keyData == Keys.Enter)
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

        private void BtnApply_Click(object sender, EventArgs e)
        {
            var qty = (int)numQuantity.Value;

            if (_type == TransactionType.StockIn)
            {
                var error = _repository.AdjustStock(_product.Id, _type, qty, txtNotes.Text.Trim());
                if (error != null)
                {
                    MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            var request = new StockOutRequest
            {
                CustomerId = customerSelect.SelectedCustomer?.Id,
                Notes = txtNotes.Text.Trim(),
                Lines = new System.Collections.Generic.List<StockOutLineItem>
                {
                    new StockOutLineItem
                    {
                        ProductId = _product.Id,
                        Quantity = qty
                    }
                }
            };

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
