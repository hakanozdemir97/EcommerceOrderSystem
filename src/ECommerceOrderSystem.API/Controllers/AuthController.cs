using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceOrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Generate JWT token for authentication
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT token</returns>
        [HttpPost("token")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GenerateToken([FromBody] TokenRequest request)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString();
                using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
                {
                    _logger.LogInformation("Token generation requested for UserId: {UserId}", request.UserId);

                    // Simple validation - in real app, check against database
                    if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Password))
                    {
                        _logger.LogWarning("Invalid token request - missing credentials");
                        return BadRequest(new { error = "UserId and Password are required" });
                    }

                    // For demo purposes, accept any non-empty credentials
                    // In real app, validate against user database
                    if (request.Password.Length < 3)
                    {
                        _logger.LogWarning("Invalid credentials for UserId: {UserId}", request.UserId);
                        return Unauthorized(new { error = "Invalid credentials" });
                    }

                    var token = GenerateJwtToken(request.UserId);
                    
                    _logger.LogInformation("Token generated successfully for UserId: {UserId}", request.UserId);
                    
                    return Ok(new TokenResponse
                    {
                        Token = token,
                        ExpiresAt = DateTime.UtcNow.AddHours(24),
                        UserId = request.UserId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while generating token");
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        private string GenerateJwtToken(string userId)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var expiryHours = int.Parse(_configuration["Jwt:ExpiryInHours"] ?? "24");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userId),
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiryHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class TokenRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}