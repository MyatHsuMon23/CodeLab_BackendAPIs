using Flights_Work_Order_APIs.Models;

namespace Flights_Work_Order_APIs.Services
{
    /// <summary>
    /// Interface for authentication operations
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user with username and decrypted password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Decrypted password</param>
        /// <returns>User if authentication successful, null otherwise</returns>
        Task<User?> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Processes login request with encrypted password
        /// </summary>
        /// <param name="request">Login request with encrypted password</param>
        /// <returns>Login response with tokens if successful, null otherwise</returns>
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}