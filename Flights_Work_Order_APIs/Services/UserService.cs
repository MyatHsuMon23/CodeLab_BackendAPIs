using Flights_Work_Order_APIs.Models;
using System.Security.Cryptography;
using System.Text;

namespace Flights_Work_Order_APIs.Services
{
    /// <summary>
    /// Service for user authentication and management
    /// </summary>
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> ValidateCredentialsAsync(string username, string password);
    }

    public class UserService : IUserService
    {
        // In a real application, this would be replaced with a database
        private readonly List<User> _users;

        public UserService()
        {
            // Initialize with some demo users
            _users = new List<User>
            {
                new User 
                { 
                    Username = "admin", 
                    PasswordHash = HashPassword("admin123"), 
                    Role = "Admin",
                    IsActive = true
                },
                new User 
                { 
                    Username = "technician", 
                    PasswordHash = HashPassword("tech123"), 
                    Role = "Technician",
                    IsActive = true
                },
                new User 
                { 
                    Username = "supervisor", 
                    PasswordHash = HashPassword("super123"), 
                    Role = "Supervisor",
                    IsActive = true
                }
            };
        }

        /// <summary>
        /// Authenticates user with username and password
        /// </summary>
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            
            if (user == null || !user.IsActive)
                return null;

            if (VerifyPassword(password, user.PasswordHash))
                return user;

            return null;
        }

        /// <summary>
        /// Gets user by username
        /// </summary>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            await Task.Delay(1); // Simulate async operation
            return _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Validates user credentials
        /// </summary>
        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await AuthenticateAsync(username, password);
            return user != null;
        }

        /// <summary>
        /// Hashes password using SHA256
        /// </summary>
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                
                return builder.ToString();
            }
        }

        /// <summary>
        /// Verifies password against hash
        /// </summary>
        private bool VerifyPassword(string password, string hash)
        {
            string hashOfInput = HashPassword(password);
            return string.Equals(hashOfInput, hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}