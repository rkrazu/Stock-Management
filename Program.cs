using System;
using System.Windows.Forms;
using Stock_Managemnet.Data;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                DatabaseInitializer.EnsureCreated();
                AuthenticationService.EnsureDefaultPassword();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not initialize the database.\r\n\r\n" + ex.Message,
                    "Stock Management",
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
