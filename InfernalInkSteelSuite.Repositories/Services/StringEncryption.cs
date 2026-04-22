using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace InfernalInkSteelSuite.Repositories.Services
{
    public static class StringEncryption
    {
        // Simple fixed key for demonstration. In a real-world app, this should be unique per installation 
        // or derived from a user-provided shop master key.
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("InF3rn4l-Ink-St33l-Su1t3-2026-Key");
        private static readonly byte[] Iv = Encoding.UTF8.GetBytes("IV-4-InF3rn4l-01");

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            if (plainText.StartsWith("ENC:")) return plainText; // Already encrypted

            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = Iv;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return "ENC:" + Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText) || !cipherText.StartsWith("ENC:")) return cipherText;

            try
            {
                var base64 = cipherText.Substring(4);
                var buffer = Convert.FromBase64String(base64);

                using var aes = Aes.Create();
                aes.Key = Key;
                aes.IV = Iv;

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(buffer);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);

                return sr.ReadToEnd();
            }
            catch
            {
                return cipherText; // Return original if decryption fails
            }
        }
    }
}
