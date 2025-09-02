using System.ComponentModel.DataAnnotations;

namespace Flights_Work_Order_APIs.Models
{
    /// <summary>
    /// Login request model with encrypted password
    /// </summary>
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; // This will be encrypted
    }

    /// <summary>
    /// Login response model containing tokens
    /// </summary>
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; } = "Bearer";
    }

    /// <summary>
    /// User model for authentication
    /// </summary>
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Stored as hash
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// JWT claims model
    /// </summary>
    public class JwtClaims
    {
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}