using System;
using System.Windows.Forms;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            UiStyles.Apply(this);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (AuthenticationService.VerifyPassword(txtPassword.Text))
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            MessageBox.Show(
                "Incorrect password.",
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            txtPassword.Clear();
            txtPassword.Focus();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
