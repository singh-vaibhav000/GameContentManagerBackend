using System.Threading.Tasks;
using System.Security.Claims;
using GameContentManager.API.Models;
using GameContentManager.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;

namespace GameContentManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IUserService userService, IConfiguration configuration)
        {
            _authService = authService;
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var (token, user) = await _authService.Authenticate(request.Email, request.Password);
            return Ok(new { Token = token, User = user });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
             {
                return Unauthorized();
             }

            var success = await _authService.ChangePassword(userId, request.CurrentPassword, request.NewPassword);
            if (!success)
                return BadRequest("Invalid current password");

            return Ok();
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<LoginResponse>> RefreshToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new LoginResponse { Token = token, User = user });
        }
    }

    public class LoginRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
} 