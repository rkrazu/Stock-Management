using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Stock_Managemnet.Services
{
    public static class BrandAssets
    {
        public const string AppDisplayName = "ELECTRONICS";
        public const string LogoFileName = "SS logo.jpeg";

        private static Image _logoImage;
        private static Icon _appIcon;
        private static bool _loadAttempted;

        public static string LogoPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", LogoFileName);

        public static Image LogoImage
        {
            get
            {
                EnsureLoaded();
                return _logoImage;
            }
        }

        public static Icon AppIcon
        {
            get
            {
                EnsureLoaded();
                return _appIcon;
            }
        }

        public static void ApplyFormIcon(Form form)
        {
            if (form == null)
                return;

            var icon = AppIcon;
            if (icon == null)
                return;

            form.Icon = (Icon)icon.Clone();
        }

        public static void ApplyLoginBranding(PictureBox logo, Label title)
        {
            if (logo != null)
            {
                logo.Image = LogoImage;
                logo.SizeMode = PictureBoxSizeMode.Zoom;
            }

            if (title != null)
            {
                title.Text = AppDisplayName;
                title.Location = new Point(132, 122);
            }
        }

        public static void DrawCompanyHeader(Graphics graphics, Rectangle bounds, Font textFont)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var logo = LogoImage;
            const int logoSize = 40;
            var textSize = graphics.MeasureString(AppDisplayName, textFont);

            if (logo != null)
            {
                var totalWidth = logoSize + 10 + textSize.Width;
                var startX = bounds.X + Math.Max(0, (bounds.Width - totalWidth) / 2);
                var centerY = bounds.Y + bounds.Height / 2f;
                var logoY = centerY - logoSize / 2f;
                graphics.DrawImage(logo, startX, logoY, logoSize, logoSize);
                graphics.DrawString(
                    AppDisplayName,
                    textFont,
                    Brushes.Black,
                    startX + logoSize + 10,
                    centerY - textSize.Height / 2f);
            }
            else
            {
                var textX = bounds.X + Math.Max(0, (bounds.Width - textSize.Width) / 2);
                var textY = bounds.Y + Math.Max(0, (bounds.Height - textSize.Height) / 2);
                graphics.DrawString(AppDisplayName, textFont, Brushes.Black, textX, textY);
            }
        }

        private static void EnsureLoaded()
        {
            if (_loadAttempted)
                return;

            _loadAttempted = true;
            if (!File.Exists(LogoPath))
                return;

            try
            {
                _logoImage = Image.FromFile(LogoPath);
                _appIcon = CreateIcon(_logoImage, 32);
            }
            catch
            {
                _logoImage = null;
                _appIcon = null;
            }
        }

        private static Icon CreateIcon(Image source, int size)
        {
            using (var bitmap = new Bitmap(source, new Size(size, size)))
            {
                var handle = bitmap.GetHicon();
                try
                {
                    using (var temp = Icon.FromHandle(handle))
                        return (Icon)temp.Clone();
                }
                finally
                {
                    DestroyIcon(handle);
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);
    }
}
