using System;
using System.Drawing;
using System.Windows.Forms;
using Stock_Managemnet.Services;
using Stock_Managemnet.Services.Licensing;

namespace Stock_Managemnet
{
    public partial class ActivationForm : Form
    {
        public ActivationForm()
        {
            InitializeComponent();
            UiStyles.Apply(this);
            BrandAssets.ApplyFormIcon(this);
            BrandAssets.ApplyLoginBranding(pbLogo, lblTitle);
            lblMachineIdValue.Text = LicenseService.CurrentMachineId;
            Text = BrandAssets.AppDisplayName + " Activation";
        }

        private void BtnCopyMachineId_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(lblMachineIdValue.Text);
                MessageBox.Show(
                    "Machine ID copied.",
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not copy Machine ID.\r\n\r\n" + ex.Message,
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void BtnActivate_Click(object sender, EventArgs e)
        {
            if (!LicenseService.TryActivate(txtActivationKey.Text, out var error))
            {
                MessageBox.Show(
                    error,
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                txtActivationKey.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
