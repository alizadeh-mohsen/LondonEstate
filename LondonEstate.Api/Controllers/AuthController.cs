using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LondonEstate.Api.Controllers
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
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest(new { message = "Email and password are required" });

                // TODO: Add user registration logic with your authentication service
                // Example: await _authService.RegisterAsync(request);

                return Ok(new { message = "User registered successfully. Please login." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Registration error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred during registration" });
            }
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest(new { message = "Email and password are required" });

                // TODO: Implement authentication logic
                // Example: var user = await _authService.AuthenticateAsync(request.Email, request.Password);
                // if (user == null)
                //     return Unauthorized(new { message = "Invalid email or password" });

                var token = GenerateJwtToken(request.Email);
                var refreshToken = GenerateRefreshToken();

                // TODO: Store refresh token in database
                // await _authService.SaveRefreshTokenAsync(user.Id, refreshToken);

                return Ok(new AuthResponse
                {
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    ExpiresIn = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60"),
                    Message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                    return BadRequest(new { message = "Refresh token is required" });

                // TODO: Validate refresh token from database
                // var isValid = await _authService.ValidateRefreshTokenAsync(request.RefreshToken);
                // if (!isValid)
                //     return Unauthorized(new { message = "Invalid or expired refresh token" });

                // TODO: Get user information from refresh token
                var email = "user@example.com"; // Replace with actual user retrieval

                var newAccessToken = GenerateJwtToken(email);
                var newRefreshToken = GenerateRefreshToken();

                // TODO: Update refresh token in database
                // await _authService.UpdateRefreshTokenAsync(userId, newRefreshToken);

                return Ok(new AuthResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresIn = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60"),
                    Message = "Token refreshed successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Refresh token error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred during token refresh" });
            }
        }

        /// <summary>
        /// Logout user
        /// </summary>
        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Logout()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userEmail))
                    return BadRequest(new { message = "User information not found" });

                // TODO: Implement logout logic (invalidate refresh tokens, cleanup sessions)
                // await _authService.LogoutAsync(userEmail);

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Logout error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred during logout" });
            }
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<UserInfo> GetCurrentUser()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(email))
                    return Unauthorized();

                return Ok(new UserInfo
                {
                    Id = userId,
                    Email = email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get current user error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        #region Helper Methods

        private string GenerateJwtToken(string email)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        #endregion
    }

    #region Request/Response Models

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public string? Message { get; set; }
    }

    public class UserInfo
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
    }

    #endregion
}
