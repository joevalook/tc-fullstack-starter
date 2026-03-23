using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkOrders.Api.Dtos;
using WorkOrders.Api.Services.Interfaces;
using Microsoft.AspNetCore.RateLimiting;

namespace WorkOrders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("api")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(
        IAuthService authService,
        ICurrentUserService currentUserService
    )
    {
        _authService = authService;
        _currentUserService = currentUserService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        if (result == null)
        {
            return BadRequest(new { message = "Email already exists." });
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        if (_currentUserService.UserId == null)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            id = _currentUserService.UserId,
            email = _currentUserService.Email,
            role = _currentUserService.Role
        });
    }
}