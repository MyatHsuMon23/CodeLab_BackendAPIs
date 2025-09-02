namespace Flights_Work_Order_APIs.Models
{
    /// <summary>
    /// User model for authentication
    /// </summary>
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Stored as plain text for this demo
        public string Role { get; set; } = "User";
    }
}