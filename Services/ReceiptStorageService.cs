using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Stock_Managemnet.Services
{
    /// <summary>
    /// Stores payment receipt photos on disk under ProgramData (outside the app install).
    /// SQL only keeps relative path + metadata so database backups stay small.
    /// </summary>
    public static class ReceiptStorageService
    {
        private const int MaxEdgePx = 1600;
        private const long JpegQuality = 75L;
        private const long MaxSourceBytes = 25L * 1024L * 1024L;

        private static readonly string[] AllowedExtensions =
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff"
        };

        public static string RootPath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "StockManagement",
                "Receipts");

        public sealed class SavedReceipt
        {
            public string RelativePath { get; set; }
            public string OriginalName { get; set; }
            public string ContentType { get; set; }
            public long SizeBytes { get; set; }
            public string Sha256 { get; set; }
        }

        public static void EnsureRootExists()
        {
            Directory.CreateDirectory(RootPath);
        }

        public static bool IsAllowedImage(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            var ext = Path.GetExtension(filePath);
            return !string.IsNullOrEmpty(ext)
                && AllowedExtensions.Contains(ext.ToLowerInvariant());
        }

        public static SavedReceipt SaveImage(string sourceFilePath, Guid paymentId, DateTime paidAt)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath) || !File.Exists(sourceFilePath))
                throw new FileNotFoundException("Receipt image was not found.", sourceFilePath);

            if (!IsAllowedImage(sourceFilePath))
                throw new InvalidOperationException("Select a photo file (JPG, PNG, BMP, GIF, or TIFF).");

            var info = new FileInfo(sourceFilePath);
            if (info.Length <= 0)
                throw new InvalidOperationException("The selected file is empty.");

            if (info.Length > MaxSourceBytes)
                throw new InvalidOperationException("Receipt photo must be 25 MB or smaller.");

            EnsureRootExists();

            var folder = Path.Combine(RootPath, paidAt.Year.ToString("0000"), paidAt.Month.ToString("00"));
            Directory.CreateDirectory(folder);

            var fileName = $"{paymentId:N}_{Guid.NewGuid():N}.jpg";
            var fullPath = Path.Combine(folder, fileName);
            var relativePath = Path.Combine(paidAt.Year.ToString("0000"), paidAt.Month.ToString("00"), fileName);

            using (var source = Image.FromFile(sourceFilePath))
            using (var resized = ResizeImage(source, MaxEdgePx))
            {
                var encoder = GetJpegEncoder();
                if (encoder == null)
                {
                    resized.Save(fullPath, ImageFormat.Jpeg);
                }
                else
                {
                    using (var encoderParams = new EncoderParameters(1))
                    {
                        encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, JpegQuality);
                        resized.Save(fullPath, encoder, encoderParams);
                    }
                }
            }

            var bytes = File.ReadAllBytes(fullPath);
            return new SavedReceipt
            {
                RelativePath = relativePath.Replace('/', '\\'),
                OriginalName = Path.GetFileName(sourceFilePath),
                ContentType = "image/jpeg",
                SizeBytes = bytes.LongLength,
                Sha256 = ComputeSha256(bytes)
            };
        }

        public static string GetFullPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            var combined = Path.GetFullPath(Path.Combine(RootPath, relativePath));
            var root = Path.GetFullPath(RootPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            if (!combined.StartsWith(root, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(combined, Path.GetFullPath(RootPath), StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Invalid receipt path.");

            return combined;
        }

        public static bool Exists(string relativePath)
        {
            try
            {
                var full = GetFullPath(relativePath);
                return !string.IsNullOrEmpty(full) && File.Exists(full);
            }
            catch
            {
                return false;
            }
        }

        public static void Open(string relativePath)
        {
            var full = GetFullPath(relativePath);
            if (string.IsNullOrEmpty(full) || !File.Exists(full))
                throw new FileNotFoundException("Receipt photo was not found on disk.", relativePath);

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = full,
                UseShellExecute = true
            });
        }

        private static Image ResizeImage(Image source, int maxEdge)
        {
            var width = source.Width;
            var height = source.Height;
            var longest = Math.Max(width, height);
            if (longest <= maxEdge)
                return new Bitmap(source);

            var scale = (float)maxEdge / longest;
            var newWidth = Math.Max(1, (int)Math.Round(width * scale));
            var newHeight = Math.Max(1, (int)Math.Round(height * scale));

            var bitmap = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(source, 0, 0, newWidth, newHeight);
            }

            return bitmap;
        }

        private static ImageCodecInfo GetJpegEncoder() =>
            ImageCodecInfo.GetImageEncoders()
                .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);

        private static string ComputeSha256(byte[] bytes)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }
    }
}
