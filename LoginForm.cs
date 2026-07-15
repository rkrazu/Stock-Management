using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Stock_Managemnet.Services;

namespace Stock_Managemnet
{
    public partial class LoginForm : Form
    {
        private const int ContentWidth = 420;
        private const int ContentHeight = 400;
        private const int LogoSize = 200;

        public LoginForm()
        {
            InitializeComponent();
            UiStyles.Apply(this);
            BrandAssets.ApplyLoginBranding(pbLogo, lblTitle);
            Text = BrandAssets.AppDisplayName;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;

            // Keep labels readable over the sketch scene.
            lblTitle.ForeColor = Color.FromArgb(30, 41, 59);
            lblPassword.ForeColor = Color.FromArgb(30, 41, 59);

            Resize += (s, e) =>
            {
                LayoutLoginContent();
                Invalidate();
            };
            LayoutLoginContent();
            Shown += LoginForm_Shown;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            LoginSceneAssets.DrawBackground(e.Graphics, ClientRectangle);
            DrawLogoWatermark(e.Graphics);
        }

        private void DrawLogoWatermark(Graphics graphics)
        {
            var logo = BrandAssets.LogoImage;
            if (logo == null || ClientSize.Width <= 0 || ClientSize.Height <= 0)
                return;

            var size = (int)(Math.Min(ClientSize.Width, ClientSize.Height) * 0.42);
            size = Math.Max(180, size);
            var bounds = new Rectangle(
                (ClientSize.Width - size) / 2,
                (ClientSize.Height - size) / 2,
                size,
                size);

            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var matrix = new ColorMatrix
            {
                Matrix33 = 0.08f
            };

            using (var attributes = new ImageAttributes())
            {
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                graphics.DrawImage(
                    logo,
                    bounds,
                    0,
                    0,
                    logo.Width,
                    logo.Height,
                    GraphicsUnit.Pixel,
                    attributes);
            }
        }

        private void LayoutLoginContent()
        {
            var left = Math.Max(0, (ClientSize.Width - ContentWidth) / 2);
            var top = Math.Max(0, (ClientSize.Height - ContentHeight) / 2);

            pbLogo.SetBounds(left + (ContentWidth - LogoSize) / 2, top, LogoSize, LogoSize);
            lblTitle.SetBounds(left, top + LogoSize + 12, ContentWidth, 34);
            lblPassword.Location = new Point(left + 30, top + LogoSize + 60);
            txtPassword.SetBounds(left + 30, top + LogoSize + 84, 360, 30);
            btnLogin.Location = new Point(left + 234, top + LogoSize + 134);
            btnCancel.Location = new Point(left + 315, top + LogoSize + 134);
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            LayoutLoginContent();
            Invalidate();
            LoginSceneAssets.PlayStartupSound();
            BeginInvoke(new Action(FocusPasswordInput));
        }

        private void FocusPasswordInput()
        {
            ActiveControl = txtPassword;
            txtPassword.FocusInput();
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
            FocusPasswordInput();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
