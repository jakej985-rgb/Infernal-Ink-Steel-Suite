using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace InfernalInkSteelSuite.Repositories.Services
{
    public class PasswordHasher
    {
        public string HashPassword(string password)
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            // return salt + hash
            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        public bool VerifyPassword(string hashedPasswordWithSalt, string password)
        {
            try
            {
                // Fallback for legacy SHA-256 hashes if encountered (optional migration path)
                if (!hashedPasswordWithSalt.Contains(':'))
                {
                    // This is legacy behavior, we might want to handle it or just fail
                    // For now, let's just use the PBKDF2 logic.
                    return false; 
                }

                var parts = hashedPasswordWithSalt.Split(':', 2);
                if (parts.Length != 2)
                {
                    return false;
                }

                var salt = Convert.FromBase64String(parts[0]);
                var hash = parts[1];

                byte[] hashedBytes = Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);

                byte[] expectedBytes = Convert.FromBase64String(hash);
                return CryptographicOperations.FixedTimeEquals(hashedBytes, expectedBytes);
            }
            catch
            {
                // If any error occurs (e.g., invalid base64 string), fail safely.
                return false;
            }
        }
    }
}
