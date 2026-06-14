using HospitalManagement.Server.Services;
using HospitalManagement.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Server.Controllers;

/// <summary>
/// Handles user registration and login.
/// All endpoints are public (no [Authorize] required).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// POST /api/auth/register
    /// Creates a new user account and returns a JWT token.
    /// Returns 409 Conflict if the username or email already exists.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(request);

        if (result is null)
        {
            _logger.LogWarning("Registration failed — username or email already exists: {Username}", request.Username);
            return Conflict(new { message = "Username or email is already taken." });
        }

        _logger.LogInformation("New user registered: {Username} ({Role})", request.Username, request.Role);
        return Ok(result);
    }

    /// <summary>
    /// POST /api/auth/login
    /// Validates credentials and returns a JWT token.
    /// Returns 401 Unauthorized if credentials are invalid.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(request);

        if (result is null)
        {
            _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
            return Unauthorized(new { message = "Invalid username or password." });
        }

        _logger.LogInformation("User logged in: {Username}", result.Username);
        return Ok(result);
    }
}
