using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Flights_Work_Order_APIs.Models;

namespace Flights_Work_Order_APIs.Services
{
    /// <summary>
    /// Service for handling JWT token operations
    /// </summary>
    public interface IJwtService
    {
        string GenerateAccessToken(JwtClaims claims);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        DateTime GetTokenExpiry();
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _accessTokenExpiryMinutes;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = _configuration["Jwt:SecretKey"] ?? "Flight_Work_Order_Secret_Key_For_JWT_Token_Generation_2024";
            _issuer = _configuration["Jwt:Issuer"] ?? "FlightWorkOrderAPI";
            _audience = _configuration["Jwt:Audience"] ?? "FlightWorkOrderClient";
            _accessTokenExpiryMinutes = int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "60");
        }

        /// <summary>
        /// Generates JWT access token with user claims
        /// </summary>
        public string GenerateAccessToken(JwtClaims claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenClaims = new[]
            {
                new Claim(ClaimTypes.Name, claims.Username),
                new Claim(ClaimTypes.Role, claims.Role),
                new Claim("UserId", claims.UserId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: tokenClaims,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generates a refresh token (simple GUID for this implementation)
        /// </summary>
        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Validates JWT token and returns claims principal
        /// </summary>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the expiry time for access tokens
        /// </summary>
        public DateTime GetTokenExpiry()
        {
            return DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes);
        }
    }
}