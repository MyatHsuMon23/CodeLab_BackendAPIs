using Microsoft.AspNetCore.Mvc;
using Flights_Work_Order_APIs.Models;
using Flights_Work_Order_APIs.Services;

namespace Flights_Work_Order_APIs.Controllers
{
    /// <summary>
    /// Authentication controller for login and token management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IUserService userService,
            IJwtService jwtService,
            IEncryptionService encryptionService,
            ILogger<AuthController> logger)
        {
            _userService = userService;
            _jwtService = jwtService;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        /// <summary>
        /// Login endpoint that accepts encrypted password
        /// </summary>
        /// <param name="request">Login request with username and encrypted password</param>
        /// <returns>JWT tokens if authentication successful</returns>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<LoginResponse>.CreateError("Invalid request data"));
                }

                // Decrypt the password
                string decryptedPassword;
                try
                {
                    decryptedPassword = _encryptionService.DecryptPassword(request.Password);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to decrypt password for user {Username}: {Error}", request.Username, ex.Message);
                    return BadRequest(ApiResponse<LoginResponse>.CreateError("Invalid password format"));
                }

                // Authenticate user
                var user = await _userService.AuthenticateAsync(request.Username, decryptedPassword);
                if (user == null)
                {
                    _logger.LogWarning("Failed login attempt for user {Username}", request.Username);
                    return Unauthorized(ApiResponse<LoginResponse>.CreateError("Invalid username or password"));
                }

                // Generate tokens
                var claims = new JwtClaims
                {
                    Username = user.Username,
                    Role = user.Role,
                    UserId = user.Username // Using username as ID for this demo
                };

                var accessToken = _jwtService.GenerateAccessToken(claims);
                var refreshToken = _jwtService.GenerateRefreshToken();
                var expiresAt = _jwtService.GetTokenExpiry();

                var loginResponse = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    TokenType = "Bearer"
                };

                _logger.LogInformation("Successful login for user {Username}", user.Username);
                return Ok(ApiResponse<LoginResponse>.CreateSuccess(loginResponse, "Login successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Username}", request.Username);
                return StatusCode(500, ApiResponse<LoginResponse>.CreateError("An error occurred during login"));
            }
        }

        /// <summary>
        /// Helper endpoint to encrypt passwords for testing (remove in production)
        /// </summary>
        /// <param name="password">Plain text password to encrypt</param>
        /// <returns>Encrypted password</returns>
        [HttpPost("encrypt-password")]
        public ActionResult<ApiResponse<string>> EncryptPassword([FromBody] string password)
        {
            try
            {
                var encryptedPassword = _encryptionService.EncryptPassword(password);
                return Ok(ApiResponse<string>.CreateSuccess(encryptedPassword, "Password encrypted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error encrypting password");
                return StatusCode(500, ApiResponse<string>.CreateError("Failed to encrypt password"));
            }
        }
    }
}