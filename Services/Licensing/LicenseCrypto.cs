using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace Stock_Managemnet.Services.Licensing
{
    public static class LicenseCrypto
    {
        private const string PublicKeyResourceName = "Stock_Managemnet.Services.Licensing.LicensePublicKey.xml";

        public static string CreateActivationKey(LicensePayload payload, string privateKeyXml)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (string.IsNullOrWhiteSpace(privateKeyXml))
                throw new ArgumentException("Private key is required.", nameof(privateKeyXml));

            var payloadJson = SerializePayload(payload);
            var signature = Sign(payloadJson, privateKeyXml);
            return EncodeToken(payloadJson, signature);
        }

        public static bool TryValidateActivationKey(string activationKey, string expectedMachineId, out LicensePayload payload, out string error)
        {
            payload = null;
            error = null;

            if (string.IsNullOrWhiteSpace(activationKey))
            {
                error = "Activation key is required.";
                return false;
            }

            if (!TryDecodeToken(activationKey.Trim(), out var payloadJson, out var signature, out error))
                return false;

            if (!Verify(payloadJson, signature, out error))
                return false;

            payload = DeserializePayload(payloadJson);
            if (payload == null)
            {
                error = "Activation key payload is invalid.";
                return false;
            }

            if (!string.Equals(payload.MachineId, expectedMachineId, StringComparison.OrdinalIgnoreCase))
            {
                error = "This activation key is for a different computer.";
                return false;
            }

            return true;
        }

        private static string SerializePayload(LicensePayload payload)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(LicensePayload));
                serializer.WriteObject(stream, payload);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private static LicensePayload DeserializePayload(string payloadJson)
        {
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(payloadJson)))
                {
                    var serializer = new DataContractJsonSerializer(typeof(LicensePayload));
                    return serializer.ReadObject(stream) as LicensePayload;
                }
            }
            catch
            {
                return null;
            }
        }

        private static string EncodeToken(string payloadJson, byte[] signature)
        {
            return "V1." +
                   ToBase64Url(Encoding.UTF8.GetBytes(payloadJson)) + "." +
                   ToBase64Url(signature);
        }

        private static bool TryDecodeToken(string token, out string payloadJson, out byte[] signature, out string error)
        {
            payloadJson = null;
            signature = null;
            error = null;

            var parts = token.Split('.');
            if (parts.Length != 3 || !string.Equals(parts[0], "V1", StringComparison.OrdinalIgnoreCase))
            {
                error = "Activation key format is invalid.";
                return false;
            }

            try
            {
                payloadJson = Encoding.UTF8.GetString(FromBase64Url(parts[1]));
                signature = FromBase64Url(parts[2]);
                return true;
            }
            catch
            {
                error = "Activation key could not be decoded.";
                return false;
            }
        }

        private static byte[] Sign(string payloadJson, string privateKeyXml)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKeyXml);
                return rsa.SignData(Encoding.UTF8.GetBytes(payloadJson), CryptoConfig.MapNameToOID("SHA256"));
            }
        }

        private static bool Verify(string payloadJson, byte[] signature, out string error)
        {
            error = null;
            var publicKeyXml = LoadPublicKeyXml();
            if (string.IsNullOrWhiteSpace(publicKeyXml))
            {
                error = "License public key is missing.";
                return false;
            }

            try
            {
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(publicKeyXml);
                    var valid = rsa.VerifyData(
                        Encoding.UTF8.GetBytes(payloadJson),
                        CryptoConfig.MapNameToOID("SHA256"),
                        signature);
                    if (!valid)
                        error = "Activation key signature is invalid.";
                    return valid;
                }
            }
            catch (Exception ex)
            {
                error = "Activation key could not be verified: " + ex.Message;
                return false;
            }
        }

        private static string LoadPublicKeyXml()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(PublicKeyResourceName))
            {
                if (stream == null)
                    return null;

                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        private static string ToBase64Url(byte[] data)
        {
            return Convert.ToBase64String(data)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static byte[] FromBase64Url(string value)
        {
            var padded = value.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }

            return Convert.FromBase64String(padded);
        }
    }
}
