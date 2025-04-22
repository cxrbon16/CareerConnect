using CareerConnect.Models.DTOs;
using CareerConnect.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = await _authService.RegisterAsync(request);
        if (user == null)
            return BadRequest("Email already in use.");

        return Ok(new { user.Id, user.Name, user.Email, user.UserType });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _authService.AuthenticateAsync(request);
        if (user == null)
            return Unauthorized("Invalid credentials.");

        return Ok(new { user.Id, user.Name, user.Email, user.UserType });
    }
}