using System;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class RecordExpenseForm : Form
    {
        private readonly StockRepository _repository;

        public RecordExpenseForm(StockRepository repository)
        {
            _repository = repository;
            InitializeComponent();
            UiStyles.Apply(this);
            Text = "Record Expense";

            dtpPaidAt.Value = DateTime.Now;
            LoadExpenseCategories();
            LoadCashAccounts();
        }

        private void LoadExpenseCategories()
        {
            var accounts = _repository.GetExpenseAccounts().ToList();
            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
            cmbCategory.DataSource = accounts;
            if (accounts.Count > 0)
                cmbCategory.SelectedIndex = 0;
        }

        private void LoadCashAccounts()
        {
            var accounts = _repository.GetCashAndBankAccounts().ToList();
            cmbCashAccount.DisplayMember = "Name";
            cmbCashAccount.ValueMember = "Id";
            cmbCashAccount.DataSource = accounts;
            if (accounts.Count > 0)
                cmbCashAccount.SelectedIndex = 0;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!(cmbCategory.SelectedValue is Guid expenseAccountId))
            {
                MessageBox.Show("Select an expense category.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(cmbCashAccount.SelectedValue is Guid cashAccountId))
            {
                MessageBox.Show("Select a cash or bank account.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var expense = new BusinessExpense
            {
                ExpenseAccountId = expenseAccountId,
                CashAccountId = cashAccountId,
                Amount = numAmount.Value,
                Reference = txtReference.Text.Trim(),
                Notes = txtNotes.Text.Trim(),
                PaidAt = dtpPaidAt.Value
            };

            var error = _repository.RecordExpense(expense);
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
