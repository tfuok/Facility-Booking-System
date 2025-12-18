using Microsoft.AspNetCore.Authorization;
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

        // ======================= LOGIN =======================
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserAccount(request.UserName, request.Password);

            // Sai username / password
            if (user == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                {
                    errorCode = 401,
                    message = "Sai username hoặc password",
                    data = null
                });
            }

            var token = GenerateJSONWebToken(user);

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Login thành công",
                data = new LoginResponse(token, user.Role.ToString())
            });
        }

        // ======================= JWT =======================
        private string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ======================= REGISTER =======================
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    errorCode = 400,
                    message = "Dữ liệu không hợp lệ",
                    data = ModelState
                });
            }

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
            {
                return BadRequest(new ApiResponse
                {
                    errorCode = 400,
                    message = "Email đã tồn tại",
                    data = null
                });
            }

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Đăng ký thành công",
                data = new
                {
                    UserId = createdUser.Id
                }
            });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPaging(
            int page = 1,
            int size = 10,
            string? keyword = null,
            Role? role = null)
        {
            var result = await _userService.GetPagingAsync(
                page,
                size,
                keyword,
                role);

            return Ok(new ApiResponse
            {
                errorCode = 0,
                message = "Success",
                data = result
            });
        }


        // ======================= DTO =======================
        public sealed record LoginRequest(string UserName, string Password);

        public sealed record LoginResponse(string Token, string Role);

        public sealed record RegisterRequest(
            string Email,
            string Password,
            string Username,
            string PhoneNumber,
            Role Role
        );
    }
}
