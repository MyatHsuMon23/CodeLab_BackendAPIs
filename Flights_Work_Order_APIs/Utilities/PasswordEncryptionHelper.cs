using Flights_Work_Order_APIs.Services;
using System.Security.Cryptography;
using System.Text;

namespace Flights_Work_Order_APIs.Utilities
{
    /// <summary>
    /// Utility class for testing password encryption/decryption
    /// This would typically be used by the client-side application
    /// </summary>
    public static class PasswordEncryptionHelper
    {
        private const string EncryptionKey = "Flight_Work_Order_[7*]";

        /// <summary>
        /// Encrypts a password for testing purposes (simulates client-side encryption)
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Base64 encoded encrypted password</returns>
        public static string EncryptPassword(string password)
        {
            var keyBytes = GetKeyBytes(EncryptionKey);
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor();
            var encryptedBytes = encryptor.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Converts the string key to a 32-byte array for AES-256
        /// </summary>
        private static byte[] GetKeyBytes(string key)
        {
            var keyBytes = new byte[32]; // AES-256 requires 32 bytes
            var sourceBytes = Encoding.UTF8.GetBytes(key);

            // Copy source bytes and pad/truncate to 32 bytes
            Array.Copy(sourceBytes, keyBytes, Math.Min(sourceBytes.Length, keyBytes.Length));

            return keyBytes;
        }
    }
}