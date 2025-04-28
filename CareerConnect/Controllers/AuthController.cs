using System.Threading.Tasks;
using CareerConnect.Models.DTOs;
using CareerConnect.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IJwtService _jwtService;

        public AuthController(AuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        // Kayıt ve login endpoint'leri anonymous erişime açık
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = await _authService.RegisterAsync(request);
            if (user == null)
                return BadRequest("Email already in use.");

            // Kayıt sonrası otomatik token üret
            var token = _jwtService.GenerateToken(user);
            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.UserType
                }
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _authService.AuthenticateAsync(request);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            var token = _jwtService.GenerateToken(user);
            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.UserType
                }
            });
        }

        // Diğer endpoint'ler artık yetkili kullanıcı gerektirir
        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var allUsers = await _authService.GetAllUsersAsync();
            return Ok(allUsers);
        }
    }
}
