using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace Stock_Managemnet.Services.Licensing
{
    public static class MachineFingerprint
    {
        public static string GetMachineId()
        {
            var raw = string.Join("|",
                Environment.MachineName ?? string.Empty,
                GetMachineGuid(),
                GetSystemDriveSerial());

            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
                return FormatMachineId(hash);
            }
        }

        private static string FormatMachineId(byte[] hash)
        {
            var hex = BitConverter.ToString(hash).Replace("-", string.Empty);
            return string.Format(
                "{0}-{1}-{2}-{3}",
                hex.Substring(0, 4),
                hex.Substring(4, 4),
                hex.Substring(8, 4),
                hex.Substring(12, 4));
        }

        private static string GetMachineGuid()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                    return key?.GetValue("MachineGuid")?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetSystemDriveSerial()
        {
            try
            {
                var systemDrive = Path.GetPathRoot(Environment.SystemDirectory)?.TrimEnd('\\');
                if (string.IsNullOrWhiteSpace(systemDrive))
                    return string.Empty;

                var driveLetter = systemDrive.TrimEnd(':');
                using (var searcher = new ManagementObjectSearcher(
                    $"SELECT VolumeSerialNumber FROM Win32_LogicalDisk WHERE DeviceID='{driveLetter}:'"))
                {
                    foreach (var item in searcher.Get())
                        return item["VolumeSerialNumber"]?.ToString() ?? string.Empty;
                }
            }
            catch
            {
                // WMI may be unavailable on some systems.
            }

            return string.Empty;
        }
    }
}
