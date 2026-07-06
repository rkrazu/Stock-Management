using System;
using System.Drawing;
using System.IO;

namespace Stock_Managemnet.Services
{
    public static class DeveloperInfo
    {
        public const string Name = "Md Razaul Karim Razu";
        public const string Email = "rkrazu2012@gmail.com";
        public const string Mobile = "01301630201";
        public const string PhotoFileName = "razaul_karim.jpg";

        public static string PhotoPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", PhotoFileName);

        public static Image LoadPhoto()
        {
            if (!File.Exists(PhotoPath))
                return null;

            try
            {
                return Image.FromFile(PhotoPath);
            }
            catch
            {
                return null;
            }
        }
    }
}
