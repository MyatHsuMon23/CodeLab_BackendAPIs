namespace Flights_Work_Order_APIs.Models
{
    /// <summary>
    /// Login response model containing JWT tokens
    /// </summary>
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}