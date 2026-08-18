using LondonEstate.Api.Dtos;
using LondonEstate.Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LondonEstate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              ITokenService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized();

            var token = _tokenService.CreateToken(user);
            var refreshToken = _tokenService.CreateRefreshToken(user);
            var refreshTokenHash = HashToken(refreshToken);
            await _tokenService.SaveRefreshTokenAsync(user.Id, refreshTokenHash, DateTime.UtcNow.AddDays(7));
            return Ok(new { token, refreshToken });

            //var token = _tokenService.CreateToken(user);
            //return Ok(new { token });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
                return BadRequest(new { message = "User with this email already exists" });

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
                return BadRequest(createResult.Errors);

            // Optionally sign in the user or return a token
            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _tokenService.RevokeRefreshTokenAsync(HashToken(token));
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(RefreshDto model)
        {
            if (string.IsNullOrWhiteSpace(model.RefreshToken))
                return BadRequest(new { message = "Refresh token is required" });

            // Validate signature
            var email = _tokenService.ValidateRefreshToken(model.RefreshToken);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { message = "Invalid or expired refresh token" });

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Unauthorized();

            // Create new tokens
            var newAccessToken = _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.CreateRefreshToken(user);
            var refreshTokenHash = HashToken(newRefreshToken);
            var expiryDate = DateTime.UtcNow.AddDays(7);

            // Save new refresh token, revoke old one
            await _tokenService.SaveRefreshTokenAsync(user.Id, refreshTokenHash, expiryDate);
            await _tokenService.RevokeRefreshTokenAsync(HashToken(model.RefreshToken));

            return Ok(new { token = newAccessToken, refreshToken = newRefreshToken });
        }

        private string HashToken(string token)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token)));
            }
        }


        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest();
            var user = await _userManager.FindByEmailAsync(email);
            return Ok(new { user.Id, user.Email, user.UserName });
        }

    }

}
