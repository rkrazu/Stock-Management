using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;

namespace Stock_Managemnet.Services
{
    public static class LoginSceneAssets
    {
        public const string BackgroundFileName = "login-nature-sketch.png";
        public const string StartupSoundFileName = "startup-chime.wav";

        private static Image _background;
        private static bool _backgroundLoadAttempted;
        private static SoundPlayer _startupPlayer;

        public static string BackgroundPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", BackgroundFileName);

        public static string StartupSoundPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", StartupSoundFileName);

        public static Image BackgroundImage
        {
            get
            {
                EnsureBackgroundLoaded();
                return _background;
            }
        }

        public static void DrawBackground(Graphics graphics, Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var image = BackgroundImage;
            if (image == null)
            {
                using (var brush = new LinearGradientBrush(
                    bounds,
                    Color.FromArgb(232, 240, 234),
                    Color.FromArgb(210, 224, 236),
                    LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(brush, bounds);
                }
                return;
            }

            var scale = Math.Max(
                bounds.Width / (float)image.Width,
                bounds.Height / (float)image.Height);
            var drawWidth = (int)Math.Ceiling(image.Width * scale);
            var drawHeight = (int)Math.Ceiling(image.Height * scale);
            var drawBounds = new Rectangle(
                bounds.X + (bounds.Width - drawWidth) / 2,
                bounds.Y + (bounds.Height - drawHeight) / 2,
                drawWidth,
                drawHeight);

            graphics.DrawImage(image, drawBounds);

            using (var veil = new SolidBrush(Color.FromArgb(70, 245, 247, 250)))
                graphics.FillRectangle(veil, bounds);
        }

        public static void PlayStartupSound()
        {
            try
            {
                if (!File.Exists(StartupSoundPath))
                    return;

                // Load from bytes so playback keeps working even if the app directory
                // is locked, and start it after UI show on a worker thread.
                if (_startupPlayer == null)
                {
                    var bytes = File.ReadAllBytes(StartupSoundPath);
                    _startupPlayer = new SoundPlayer(new MemoryStream(bytes));
                    _startupPlayer.Load();
                }

                _startupPlayer.Play();
            }
            catch
            {
                // Startup sound is non-critical.
            }
        }

        private static void EnsureBackgroundLoaded()
        {
            if (_backgroundLoadAttempted)
                return;

            _backgroundLoadAttempted = true;
            if (!File.Exists(BackgroundPath))
                return;

            try
            {
                _background = Image.FromFile(BackgroundPath);
            }
            catch
            {
                _background = null;
            }
        }
    }
}
