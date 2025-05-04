using System.Linq;
using System.Threading.Tasks;
using CareerConnect.Models.DTOs;
using CareerConnect.Models;
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

        /// <summary>
        /// Yeni kullanıcı kaydı. Front-end'den name, email, password, userType ("JOB_SEEKER" | "EMLOYER") gelir.
        /// Dönen JSON: { token, user: { id, name, email, type: "job-seeker" | "employer" } }
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.RegisterAsync(request);
            if (user == null)
                return BadRequest(new { message = "Email already in use." });

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    type = user.UserType == UserType.JOB_SEEKER ? "job-seeker" : "employer"
                }
            });
        }

        /// <summary>
        /// Kullanıcı girişi. Front-end'den email, password gelir.
        /// Dönen JSON: { token, user: { id, name, email, type } }
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.AuthenticateAsync(request);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials." });

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    type = user.UserType == UserType.JOB_SEEKER ? "job-seeker" : "employer"
                }
            });
        }

        /// <summary>
        /// Tüm kullanıcıları getirir. Yetkili erişim gerektirir.
        /// Dönen JSON: [{ id, name, email, type }, ...]
        /// </summary>
        [Authorize]
        [HttpGet("all")] // şuanlık böyle normalde /admin olmalı
        public async Task<IActionResult> GetAll()
        {
            var allUsers = await _authService.GetAllUsersAsync();
            var result = allUsers.Select(u => new
            {
                id = u.Id,
                name = u.Name,
                email = u.Email,
                type = u.UserType == UserType.JOB_SEEKER ? "job-seeker" : "employer"
            });
            return Ok(result);
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var name = User.FindFirstValue("name");
            var userType = User.FindFirstValue("UserType");

            return Ok(new
            {
                id = int.Parse(userId),
                email,
                name,
                type = userType == "EMPLOYER" ? "employer" : "job-seeker"
            });
        }

        [Authorize]
        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfileById(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                //experience = user.Experience, // varsa
                type = user.UserType == UserType.JOB_SEEKER ? "job-seeker" : "employer"
            });
        }

    }
}
