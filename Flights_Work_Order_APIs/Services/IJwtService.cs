using Flights_Work_Order_APIs.Models;

namespace Flights_Work_Order_APIs.Services
{
    /// <summary>
    /// Interface for JWT token operations
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a new access token for the user
        /// </summary>
        string GenerateAccessToken(User user);

        /// <summary>
        /// Generates a new refresh token
        /// </summary>
        string GenerateRefreshToken();

        /// <summary>
        /// Validates and parses a JWT token
        /// </summary>
        /// <param name="token">JWT token to validate</param>
        /// <returns>Username if valid, null if invalid</returns>
        string? ValidateToken(string token);
    }
}