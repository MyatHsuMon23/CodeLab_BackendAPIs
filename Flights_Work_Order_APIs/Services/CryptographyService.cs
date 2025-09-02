using System.Security.Cryptography;
using System.Text;

namespace Flights_Work_Order_APIs.Services
{
    /// <summary>
    /// Service for handling password encryption/decryption using AES
    /// </summary>
    public class CryptographyService : ICryptographyService
    {
        private const string EncryptionKey = "Flight_Work_Order_[7*]";

        /// <summary>
        /// Decrypts password using AES with the predefined key
        /// </summary>
        /// <param name="encryptedPassword">Base64 encoded encrypted password</param>
        /// <returns>Decrypted password</returns>
        public string DecryptPassword(string encryptedPassword)
        {
            try
            {
                // Convert the encryption key to a 32-byte key for AES-256
                var keyBytes = GetKeyBytes(EncryptionKey);
                
                // Decode the base64 encrypted password
                var encryptedBytes = Convert.FromBase64String(encryptedPassword);
                
                using var aes = Aes.Create();
                aes.Key = keyBytes;
                aes.Mode = CipherMode.ECB; // Using ECB for simplicity (in production, use CBC with IV)
                aes.Padding = PaddingMode.PKCS7;
                
                using var decryptor = aes.CreateDecryptor();
                var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid encrypted password format", ex);
            }
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