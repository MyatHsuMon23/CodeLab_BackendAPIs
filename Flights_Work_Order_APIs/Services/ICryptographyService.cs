namespace Flights_Work_Order_APIs.Services
{
    /// <summary>
    /// Interface for cryptography operations
    /// </summary>
    public interface ICryptographyService
    {
        /// <summary>
        /// Decrypts password using the Flight Work Order key
        /// </summary>
        /// <param name="encryptedPassword">Encrypted password from client</param>
        /// <returns>Decrypted password</returns>
        string DecryptPassword(string encryptedPassword);
    }
}