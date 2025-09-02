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
}