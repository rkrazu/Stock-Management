using System;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class BankAccountEditForm : Form
    {
        private readonly StockRepository _repository;
        private readonly BankAccount _bank;
        private readonly bool _isNew;

        public BankAccountEditForm(StockRepository repository, BankAccount bank = null)
        {
            _repository = repository;
            _isNew = bank == null;
            _bank = bank ?? new BankAccount();
            InitializeComponent();
            UiStyles.Apply(this);
            Text = _isNew ? "Add Bank Account" : "Edit Bank Account";
            btnSave.Text = _isNew ? "Add" : "Save";

            if (!_isNew)
            {
                txtName.Text = _bank.Name;
                txtAccountNumber.Text = _bank.AccountNumber;
                txtBranch.Text = _bank.Branch;
                txtNotes.Text = _bank.Notes;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            _bank.Name = txtName.Text.Trim();
            _bank.AccountNumber = txtAccountNumber.Text.Trim();
            _bank.Branch = txtBranch.Text.Trim();
            _bank.Notes = txtNotes.Text.Trim();

            var error = _isNew
                ? _repository.AddBankAccount(_bank)
                : _repository.UpdateBankAccount(_bank);

            if (error != null)
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
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
