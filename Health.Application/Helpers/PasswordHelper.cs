using BCrypt.Net;

namespace Health.Application.Services.Security
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Hashes a password using BCrypt (automatically includes salt).
        /// </summary>
        public static string HashPassword(string password)
        {
            // Work factor determines hashing cost (default: 10)
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies a password against a stored BCrypt hash.
        /// </summary>
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }
    }
}
