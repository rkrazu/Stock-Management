using System;
using System.Security.Cryptography;
using Stock_Managemnet.Data;

namespace Stock_Managemnet.Services
{
    public static class AuthenticationService
    {
        public const string DefaultPassword = "123456";

        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        public static void EnsureDefaultPassword()
        {
            if (CredentialStore.Exists())
                return;

            SetPassword(DefaultPassword, enforcePolicy: false);
        }

        public static bool VerifyPassword(string password)
        {
            var stored = CredentialStore.Get();
            if (stored == null || string.IsNullOrEmpty(password))
                return false;

            return VerifyHash(password, stored.Hash, stored.Salt);
        }

        public static string ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (!VerifyPassword(currentPassword))
                return "Current password is incorrect.";

            if (string.IsNullOrEmpty(newPassword))
                return "New password is required.";

            if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
                return "New password and confirmation do not match.";

            if (string.Equals(currentPassword, newPassword, StringComparison.Ordinal))
                return "New password must be different from the current password.";

            var policyError = PasswordPolicy.Validate(newPassword);
            if (policyError != null)
                return policyError;

            SetPassword(newPassword, enforcePolicy: true);
            return null;
        }

        private static void SetPassword(string password, bool enforcePolicy)
        {
            if (enforcePolicy)
            {
                var policyError = PasswordPolicy.Validate(password);
                if (policyError != null)
                    throw new ArgumentException(policyError);
            }

            var salt = GenerateSalt();
            var hash = ComputeHash(password, salt);
            CredentialStore.Save(Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        private static bool VerifyHash(string password, string storedHashBase64, string storedSaltBase64)
        {
            var storedHash = Convert.FromBase64String(storedHashBase64);
            var salt = Convert.FromBase64String(storedSaltBase64);
            var computedHash = ComputeHash(password, salt);

            if (computedHash.Length != storedHash.Length)
                return false;

            var diff = 0;
            for (var i = 0; i < storedHash.Length; i++)
                diff |= storedHash[i] ^ computedHash[i];

            return diff == 0;
        }

        private static byte[] ComputeHash(string password, byte[] salt)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
                return deriveBytes.GetBytes(HashSize);
        }

        private static byte[] GenerateSalt()
        {
            var salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(salt);

            return salt;
        }
    }
}
