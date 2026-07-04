using System;
using System.Windows.Forms;
using Stock_Managemnet.Data;
using Stock_Managemnet.Services;
using Stock_Managemnet.Services.Licensing;

namespace Stock_Managemnet
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!LicenseService.IsActivated())
            {
                using (var activation = new ActivationForm())
                {
                    if (activation.ShowDialog() != DialogResult.OK)
                        return;
                }
            }

            try
            {
                DatabaseInitializer.EnsureCreated();
                AuthenticationService.EnsureDefaultPassword();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not initialize the database.\r\n\r\n" + ex.Message,
                    BrandAssets.AppDisplayName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            using (var login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                    return;
            }

            Application.Run(new Form1());
        }
    }
}
