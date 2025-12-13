using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repositories.Models;
using Repositories.Models.Enums;
using Services.UserService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FacilityBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserService _userService;

        public UserController(IConfiguration config, UserService userService)
        {
            _config = config;
            _userService = userService;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _userService.GetUserAccount(request.UserName, request.Password);

            if (user == null || user.Result == null)
                return Unauthorized();

            var token = GenerateJSONWebToken(user.Result);
            LoginResponse loginResponse = new LoginResponse(token, user.Result.Role.ToString());
            return Ok(loginResponse);
        }

        private string GenerateJSONWebToken(User systemUserAccount)
        {
            if (systemUserAccount.Role.Equals("Student"))
            {
                return string.Empty;
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
    {
        new(ClaimTypes.Email, systemUserAccount.Email),
        new(ClaimTypes.Role, systemUserAccount.Role.ToString()),
        new(ClaimTypes.NameIdentifier, systemUserAccount.Id.ToString()),
        new("UserId", systemUserAccount.Id.ToString()),
        new("sub", systemUserAccount.Id.ToString()),
    };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public sealed record LoginRequest(string UserName, string Password);

        public sealed record LoginResponse(string Token, string role);


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newUser = new User
            {
                Email = request.Email,
                Password = request.Password,
                Username = request.Username,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role
            };

            var createdUser = await _userService.Register(newUser);

            if (createdUser == null)
                return BadRequest("Email already exists.");

            return Ok(new
            {
                Message = "User registered successfully",
                UserId = createdUser.Id
            });
        }

        public sealed record RegisterRequest(
            string Email,
            string Password,
            string Username,
            string PhoneNumber,
            Role Role
        );

    }
}

