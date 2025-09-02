using System.Security.Cryptography;
using System.Text;

namespace Flights_Work_Order_APIs.Services
{
    /// <summary>
    /// Service for encrypting and decrypting passwords using AES encryption
    /// </summary>
    public interface IEncryptionService
    {
        string DecryptPassword(string encryptedPassword);
        string EncryptPassword(string password);
    }

    public class EncryptionService : IEncryptionService
    {
        private readonly string _encryptionKey = "Flight_Work_Order_[7*]";

        /// <summary>
        /// Decrypts password using the predefined key
        /// </summary>
        /// <param name="encryptedPassword">Base64 encoded encrypted password</param>
        /// <returns>Decrypted password</returns>
        public string DecryptPassword(string encryptedPassword)
        {
            try
            {
                // Convert the encryption key to bytes and ensure it's 32 bytes for AES-256
                byte[] keyBytes = GetKeyBytes(_encryptionKey);
                
                // Decode the base64 encrypted data
                byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);
                
                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.Mode = CipherMode.ECB; // Using ECB mode for simplicity
                    aes.Padding = PaddingMode.PKCS7;
                    
                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    {
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to decrypt password: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Encrypts password using the predefined key (for testing purposes)
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Base64 encoded encrypted password</returns>
        public string EncryptPassword(string password)
        {
            try
            {
                byte[] keyBytes = GetKeyBytes(_encryptionKey);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                
                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.PKCS7;
                    
                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    {
                        byte[] encryptedBytes = encryptor.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);
                        return Convert.ToBase64String(encryptedBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to encrypt password: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts the string key to a 32-byte array for AES-256
        /// </summary>
        private byte[] GetKeyBytes(string key)
        {
            byte[] keyBytes = new byte[32]; // 256 bits
            byte[] sourceBytes = Encoding.UTF8.GetBytes(key);
            
            // Copy the source bytes and pad with zeros if necessary
            Array.Copy(sourceBytes, 0, keyBytes, 0, Math.Min(sourceBytes.Length, keyBytes.Length));
            
            return keyBytes;
        }
    }
}