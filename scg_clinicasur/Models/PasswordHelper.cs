using System.Security.Cryptography;
using System.Text;

namespace scg_clinicasur.Models
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var salt = GenerateSalt();
                var combinedPassword = password + salt;
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedPassword));
                var hash = Convert.ToBase64String(hashedBytes);
                return $"{hash}:{salt}"; // Retornamos el hash combinado con el salt
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            var parts = storedPassword.Split(':');
            if (parts.Length != 2) return false;

            var hash = parts[0];
            var salt = parts[1];

            using (var sha256 = SHA256.Create())
            {
                var combinedPassword = enteredPassword + salt;
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedPassword));
                var enteredHash = Convert.ToBase64String(hashedBytes);

                return enteredHash == hash; // Comparamos el hash generado con el almacenado
            }
        }

        private static string GenerateSalt()
        {
            var saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }
    }
}
