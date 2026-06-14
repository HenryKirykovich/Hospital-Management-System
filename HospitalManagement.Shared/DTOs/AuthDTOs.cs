namespace HospitalManagement.Shared.DTOs;

/// <summary>Request payload for user login</summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>Request payload for user registration</summary>
public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    /// <summary>Role: Admin | Doctor | Nurse | Patient</summary>
    public string Role { get; set; } = "Patient";
}

/// <summary>Response returned after successful authentication</summary>
public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
