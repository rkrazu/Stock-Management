using System;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Models;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class TransferFundsForm : Form
    {
        private readonly StockRepository _repository;
        private readonly Guid? _preferredFromAccountId;

        public TransferFundsForm(StockRepository repository, Guid? preferredFromAccountId = null)
        {
            _repository = repository;
            _preferredFromAccountId = preferredFromAccountId;
            InitializeComponent();
            UiStyles.Apply(this);
            Text = "Transfer Funds";

            dtpTransferredAt.Value = DateTime.Now;
            LoadAccounts();
        }

        private void LoadAccounts()
        {
            var accounts = _repository.GetCashAndBankAccounts().ToList();
            cmbFromAccount.DisplayMember = "Name";
            cmbFromAccount.ValueMember = "Id";
            cmbFromAccount.DataSource = accounts.ToList();

            cmbToAccount.DisplayMember = "Name";
            cmbToAccount.ValueMember = "Id";
            cmbToAccount.DataSource = accounts.ToList();

            if (accounts.Count == 0)
                return;

            if (_preferredFromAccountId.HasValue &&
                accounts.Any(a => a.Id == _preferredFromAccountId.Value))
            {
                cmbFromAccount.SelectedValue = _preferredFromAccountId.Value;
            }
            else
            {
                cmbFromAccount.SelectedIndex = 0;
            }

            if (accounts.Count > 1)
            {
                var fromId = cmbFromAccount.SelectedValue as Guid? ?? Guid.Empty;
                var toIndex = accounts.FindIndex(a => a.Id != fromId);
                cmbToAccount.SelectedIndex = toIndex >= 0 ? toIndex : 0;
            }
            else
            {
                cmbToAccount.SelectedIndex = 0;
            }

            UpdateFromBalance();
        }

        private void CmbFromAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFromBalance();
        }

        private void UpdateFromBalance()
        {
            if (!(cmbFromAccount.SelectedValue is Guid accountId) || accountId == Guid.Empty)
            {
                lblFromBalance.Text = "Balance: —";
                return;
            }

            var balance = _repository.GetCashOrBankDisplayBalance(accountId);
            lblFromBalance.Text = $"Balance: {balance:C2}";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!(cmbFromAccount.SelectedValue is Guid fromAccountId) || fromAccountId == Guid.Empty)
            {
                MessageBox.Show("Select a From account.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(cmbToAccount.SelectedValue is Guid toAccountId) || toAccountId == Guid.Empty)
            {
                MessageBox.Show("Select a To account.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (fromAccountId == toAccountId)
            {
                MessageBox.Show("From and To accounts must be different.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var error = _repository.TransferBetweenAccounts(
                fromAccountId,
                toAccountId,
                numAmount.Value,
                dtpTransferredAt.Value,
                txtReference.Text,
                txtNotes.Text);

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
