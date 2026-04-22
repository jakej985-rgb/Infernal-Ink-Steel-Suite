using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace InfernalInkSteelSuite.Repositories.Services
{
    public static class StringEncryption
    {
        // Legacy V1 (Hardcoded - DON'T USE FOR NEW ENCRYPTIONS)
        private static readonly byte[] LegacyKey = Encoding.UTF8.GetBytes("InF3rn4l-Ink-St33l-Su1t3-2026-Key");
        private static readonly byte[] LegacyIv = Encoding.UTF8.GetBytes("IV-4-InF3rn4l-01");

        private static byte[] GetMasterKey()
        {
            var keyStr = Environment.GetEnvironmentVariable("SHOP_MASTER_KEY");
            if (string.IsNullOrWhiteSpace(keyStr))
            {
                // Fallback to legacy key if master key not set (Dev only)
                return LegacyKey;
            }

            // Derive a 32-byte key from the environment string
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(keyStr));
        }

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            if (plainText.StartsWith("ENC:") || plainText.StartsWith("ENC2:")) return plainText;

            using var aes = Aes.Create();
            aes.Key = GetMasterKey();
            aes.GenerateIV(); // Use a random IV per encryption (Fix C1)

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            
            // Prepend IV to the stream so it can be recovered during decryption
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return "ENC2:" + Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            if (cipherText.StartsWith("ENC2:"))
            {
                try
                {
                    var buffer = Convert.FromBase64String(cipherText.Substring(5));
                    using var aes = Aes.Create();
                    aes.Key = GetMasterKey();
                    
                    var iv = new byte[aes.BlockSize / 8];
                    Array.Copy(buffer, 0, iv, 0, iv.Length);
                    aes.IV = iv;

                    using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length);
                    using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                    using var sr = new StreamReader(cs);

                    return sr.ReadToEnd();
                }
                catch { return cipherText; }
            }

            if (cipherText.StartsWith("ENC:"))
            {
                try
                {
                    var base64 = cipherText.Substring(4);
                    var buffer = Convert.FromBase64String(base64);

                    using var aes = Aes.Create();
                    aes.Key = LegacyKey;
                    aes.IV = LegacyIv;

                    using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using var ms = new MemoryStream(buffer);
                    using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                    using var sr = new StreamReader(cs);

                    return sr.ReadToEnd();
                }
                catch { return cipherText; }
            }

            return cipherText;
        }
    }
}
