using System;
using System.IO;

namespace Stock_Managemnet.Services.Licensing
{
    public static class LicenseService
    {
        private const string LicenseFileName = "license.key";

        public static string CurrentMachineId => MachineFingerprint.GetMachineId();

        public static string LicenseFilePath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "SS Electronics",
                LicenseFileName);

        public static bool IsActivated()
        {
            return TryGetActivatedCustomer(out _);
        }

        public static bool TryGetActivatedCustomer(out string customerName)
        {
            customerName = null;
            var activationKey = LoadStoredActivationKey();
            if (string.IsNullOrWhiteSpace(activationKey))
                return false;

            if (!LicenseCrypto.TryValidateActivationKey(
                activationKey,
                CurrentMachineId,
                out var payload,
                out _))
            {
                return false;
            }

            customerName = payload.Customer;
            return true;
        }

        public static bool TryActivate(string activationKey, out string error)
        {
            error = null;
            if (!LicenseCrypto.TryValidateActivationKey(
                activationKey,
                CurrentMachineId,
                out var payload,
                out error))
            {
                return false;
            }

            try
            {
                var directory = Path.GetDirectoryName(LicenseFilePath);
                if (!string.IsNullOrWhiteSpace(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllText(LicenseFilePath, activationKey.Trim());
                return true;
            }
            catch (Exception ex)
            {
                error = "Could not save the license: " + ex.Message;
                return false;
            }
        }

        private static string LoadStoredActivationKey()
        {
            try
            {
                return File.Exists(LicenseFilePath) ? File.ReadAllText(LicenseFilePath).Trim() : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
