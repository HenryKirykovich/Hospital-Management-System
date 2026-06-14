using HospitalManagement.Shared.DTOs;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Contract for authentication operations: register, login, JWT generation.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user. Returns null if the username or email already exists.
    /// </summary>
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Validates credentials and returns a JWT token on success.
    /// Returns null if credentials are invalid.
    /// </summary>
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}
