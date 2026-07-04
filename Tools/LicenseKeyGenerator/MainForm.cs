using System;
using System.Windows.Forms;
using Stock_Managemnet.Services.Licensing;

namespace LicenseKeyGenerator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            DeveloperKeys.EnsureCreated(out _);
        }

        private void BtnCreateKeys_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(DeveloperKeys.PrivateKeyPath))
            {
                var confirm = MessageBox.Show(
                    "Developer keys already exist. Create a new key pair?\r\n\r\n" +
                    "Old activation keys will stop working until you rebuild the main app and re-issue keys to customers.",
                    Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes)
                    return;

                System.IO.File.Delete(DeveloperKeys.PrivateKeyPath);
            }

            if (!DeveloperKeys.EnsureCreated(out var message))
            {
                MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            txtActivationKey.Clear();

            if (string.IsNullOrWhiteSpace(txtCustomer.Text))
            {
                MessageBox.Show("Enter the customer name.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomer.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMachineId.Text))
            {
                MessageBox.Show("Enter the customer's Machine ID.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMachineId.Focus();
                return;
            }

            try
            {
                var payload = new LicensePayload
                {
                    Customer = txtCustomer.Text.Trim(),
                    MachineId = txtMachineId.Text.Trim().ToUpperInvariant(),
                    IssuedUtc = DateTime.UtcNow.ToString("o"),
                    Version = 1
                };

                var key = LicenseCrypto.CreateActivationKey(payload, DeveloperKeys.LoadPrivateKeyXml());
                txtActivationKey.Text = key;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtActivationKey.Text))
                return;

            try
            {
                Clipboard.SetText(txtActivationKey.Text);
                MessageBox.Show("Activation key copied.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
