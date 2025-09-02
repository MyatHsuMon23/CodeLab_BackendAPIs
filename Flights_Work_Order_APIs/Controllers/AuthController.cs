using Flights_Work_Order_APIs.Models;
using Flights_Work_Order_APIs.Services;
using Microsoft.AspNetCore.Mvc;

namespace Flights_Work_Order_APIs.Controllers
{
    /// <summary>
    /// Authentication controller for login operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates user and returns JWT tokens
        /// </summary>
        /// <param name="request">Login request with username and encrypted password</param>
        /// <returns>Login response with access and refresh tokens</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data"));
                }

                var result = await _authService.LoginAsync(request);
                if (result == null)
                {
                    _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid username or password"));
                }

                _logger.LogInformation("Successful login for username: {Username}", request.Username);
                return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Login successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username: {Username}", request.Username);
                return BadRequest(ApiResponse<object>.ErrorResponse("Login failed due to invalid encrypted password format"));
            }
        }
    }
}