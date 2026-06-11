using System.Linq;
using System.Text;

namespace Stock_Managemnet.Services
{
    public static class PasswordPolicy
    {
        public const int MinimumLength = 8;

        public static string RequirementsText =>
            "Password must be at least 8 characters and include uppercase, lowercase, a number, and a special character.";

        public static string Validate(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "Password is required.";

            if (password.Length < MinimumLength)
                return $"Password must be at least {MinimumLength} characters long.";

            if (!password.Any(char.IsUpper))
                return "Password must contain at least one uppercase letter.";

            if (!password.Any(char.IsLower))
                return "Password must contain at least one lowercase letter.";

            if (!password.Any(char.IsDigit))
                return "Password must contain at least one number.";

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                return "Password must contain at least one special character.";

            return null;
        }
    }
}
