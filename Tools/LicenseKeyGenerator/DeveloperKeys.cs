using System;
using System.IO;
using System.Security.Cryptography;

namespace LicenseKeyGenerator
{
    internal static class DeveloperKeys
    {
        public static string PrivateKeyPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keys", "private.xml");

        public static string PublicKeyPath =>
            Path.GetFullPath(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", "..", "Services", "Licensing", "LicensePublicKey.xml"));

        public static bool EnsureCreated(out string message)
        {
            message = null;
            if (File.Exists(PrivateKeyPath))
            {
                message = "Developer private key already exists.";
                return true;
            }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PrivateKeyPath));
                Directory.CreateDirectory(Path.GetDirectoryName(PublicKeyPath));

                using (var rsa = new RSACryptoServiceProvider(2048))
                {
                    File.WriteAllText(PrivateKeyPath, rsa.ToXmlString(true));
                    File.WriteAllText(PublicKeyPath, rsa.ToXmlString(false));
                }

                message =
                    "New license keys were created.\r\n\r\n" +
                    "Rebuild the main ELECTRONICS application before generating customer activation keys.";
                return true;
            }
            catch (Exception ex)
            {
                message = "Could not create license keys: " + ex.Message;
                return false;
            }
        }

        public static string LoadPrivateKeyXml()
        {
            if (!File.Exists(PrivateKeyPath))
                throw new InvalidOperationException(
                    "Developer private key was not found. Click \"Create Developer Keys\" first.");

            return File.ReadAllText(PrivateKeyPath);
        }
    }
}
