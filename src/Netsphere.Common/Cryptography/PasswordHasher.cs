using System;
using System.Security.Cryptography;

namespace Netsphere.Common.Cryptography
{
    public static class PasswordHasher
    {
        public const int Iterations = 24000;

        public static bool IsPasswordValid(string password, string hash, string salt)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash) || string.IsNullOrWhiteSpace(salt))
                return false;

            // Validate password
            var actualPassword = Convert.FromBase64String(hash);
            byte[] passwordGuess;

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), Iterations))
                passwordGuess = pbkdf2.GetBytes(24);

            var difference = (uint)passwordGuess.Length ^ (uint)actualPassword.Length;
            for (var i = 0; i < passwordGuess.Length && i < actualPassword.Length; i++)
                difference |= (uint)(passwordGuess[i] ^ actualPassword[i]);

            return difference == 0;
        }

        public static (string hash, string salt) Hash(string password)
        {
            var salt = new byte[24];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            var hash = new byte[24];
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
                hash = pbkdf2.GetBytes(24);

            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }
    }
}
