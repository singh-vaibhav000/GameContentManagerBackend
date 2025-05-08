using System.Security.Claims;
using GameContentManager.API.Models;
using GameContentManager.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GameContentManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var user = new User
            {
                Email = request.Email,
                PasswordHash = request.Password,
                Roles = request.Roles
            };

            var createdUser = await _userService.CreateUserAsync(user, userId);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var user = new User
            {
                Id = id, 
                Email = request.Email,
                Roles = request.Roles
            };


            await _userService.UpdateUserAsync(id, user, userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            await _userService.DeleteUserAsync(id, userId);
            return NoContent();
        }

        [HttpPost("{id}/roles")]
        public async Task<IActionResult> AssignRoles(string id, [FromBody] AssignRolesRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            await _userService.AssignRolesAsync(id, request.Roles, userId);
            return NoContent();
        }

        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] AdminChangePasswordRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            await _userService.ChangePasswordAsync(id, request.NewPassword, userId);
            return NoContent();
        }
    }

    public class CreateUserRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public List<UserRole> Roles { get; set; } = new();
    }

    public class UpdateUserRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public List<UserRole> Roles { get; set; } = new();
    }

    public class AssignRolesRequest
    {
        [Required]
        public List<UserRole> Roles { get; set; } = new();
    }

    public class AdminChangePasswordRequest
    {
        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
} 