using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HospitalManagement.Server.Data;
using HospitalManagement.Shared.DTOs;
using HospitalManagement.Shared.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Handles user registration, login, and JWT token generation.
/// Passwords are hashed with BCrypt — plain text is never stored.
/// </summary>
public class AuthService : IAuthService
{
    private readonly MongoDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(MongoDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    /// <inheritdoc/>
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        // Check whether the username or email is already taken
        var existingUser = await _db.Users
            .Find(u => u.Username == request.Username || u.Email == request.Email)
            .FirstOrDefaultAsync();

        if (existingUser is not null)
            return null;

        // Hash the password with BCrypt (work factor 12)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

        var newUser = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            FullName = request.FullName,
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Users.InsertOneAsync(newUser);

        return BuildAuthResponse(newUser);
    }

    /// <inheritdoc/>
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users
            .Find(u => u.Username == request.Username && u.IsActive)
            .FirstOrDefaultAsync();

        if (user is null)
            return null;

        // Verify the provided password against the stored BCrypt hash
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        // Update last login timestamp
        var update = Builders<User>.Update.Set(u => u.LastLogin, DateTime.UtcNow);
        await _db.Users.UpdateOneAsync(u => u.Id == user.Id, update);

        return BuildAuthResponse(user);
    }

    /// <summary>
    /// Creates a signed JWT token and packages it in an AuthResponse.
    /// </summary>
    private AuthResponse BuildAuthResponse(User user)
    {
        var jwtKey = _config["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key is not configured.");
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];
        var expiryHours = int.Parse(_config["Jwt:ExpiryHours"] ?? "8");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddHours(expiryHours);

        // Claims embedded in the token — readable by the client without DB lookup
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id!),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("fullName", user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiry,
            signingCredentials: credentials);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.Id!,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            ExpiresAt = expiry
        };
    }
}
