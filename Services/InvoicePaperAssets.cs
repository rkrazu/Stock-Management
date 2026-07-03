using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Stock_Managemnet.Services
{
    public static class InvoicePaperAssets
    {
        public const string PadFileName = "Pad.jpg";

        private static Image _padImage;
        private static bool _loadAttempted;

        public static bool HasPad => PadImage != null;

        public static Image PadImage
        {
            get
            {
                EnsureLoaded();
                return _padImage;
            }
        }

        public static Size PageSize
        {
            get
            {
                var pad = PadImage;
                return pad != null ? pad.Size : new Size(794, 1123);
            }
        }

        public static Size GetLogicalPageSize(int targetWidth)
        {
            targetWidth = Math.Max(320, targetWidth);
            var pad = PadImage;
            if (pad == null)
            {
                var height = (int)Math.Round(targetWidth * (297.0 / 210.0));
                return new Size(targetWidth, height);
            }

            var scale = targetWidth / (float)pad.Width;
            return new Size(targetWidth, Math.Max(1, (int)Math.Round(pad.Height * scale)));
        }

        public static string PadPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", PadFileName);

        public static Rectangle GetContentBounds(int pageWidth, int pageHeight)
        {
            var left = (int)Math.Round(pageWidth * 0.05);
            var top = (int)Math.Round(pageHeight * 0.22);
            var right = (int)Math.Round(pageWidth * 0.05);
            var bottom = (int)Math.Round(pageHeight * 0.07);
            var width = Math.Max(1, pageWidth - left - right);
            var height = Math.Max(1, pageHeight - top - bottom);
            return new Rectangle(left, top, width, height);
        }

        public static void DrawPageBackground(Graphics graphics, Rectangle pageBounds)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var pad = PadImage;
            if (pad != null)
            {
                graphics.DrawImage(pad, pageBounds);
                return;
            }

            using (var brush = new SolidBrush(Color.White))
                graphics.FillRectangle(brush, pageBounds);
        }

        private static void EnsureLoaded()
        {
            if (_loadAttempted)
                return;

            _loadAttempted = true;
            if (!File.Exists(PadPath))
                return;

            try
            {
                _padImage = Image.FromFile(PadPath);
            }
            catch
            {
                _padImage = null;
            }
        }
    }
}
