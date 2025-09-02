using Flights_Work_Order_APIs.Models;

namespace Flights_Work_Order_APIs.Services
{
    /// <summary>
    /// Service for handling user authentication
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly IJwtService _jwtService;

        // Demo users - in a real application, this would be from a database
        private readonly List<User> _users = new()
        {
            new User { Username = "admin", Password = "admin123", Role = "Admin" },
            new User { Username = "user", Password = "user123", Role = "User" },
            new User { Username = "test", Password = "test123", Role = "User" }
        };

        public AuthService(ICryptographyService cryptographyService, IJwtService jwtService)
        {
            _cryptographyService = cryptographyService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Authenticates a user with username and decrypted password
        /// </summary>
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            // Simulate async operation
            await Task.Delay(10);

            var user = _users.FirstOrDefault(u => 
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                u.Password == password);

            return user;
        }

        /// <summary>
        /// Processes login request with encrypted password
        /// </summary>
        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            try
            {
                // Decrypt the password
                var decryptedPassword = _cryptographyService.DecryptPassword(request.Password);

                // Authenticate user
                var user = await AuthenticateAsync(request.Username, decryptedPassword);
                if (user == null)
                {
                    return null;
                }

                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                return new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    Username = user.Username
                };
            }
            catch (Exception)
            {
                // Invalid encrypted password or other error
                return null;
            }
        }
    }
}