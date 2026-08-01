using System.Reflection;

namespace Stock_Managemnet.Services
{
    public static class AppVersion
    {
        public const string DisplayVersion = "1.8";

        public static string ProductName => BrandAssets.AppDisplayName;

        public static string DisplayText => $"{ProductName} {DisplayVersion}";

        public static string AssemblyVersionText
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return version != null ? version.ToString() : DisplayVersion;
            }
        }
    }
}
