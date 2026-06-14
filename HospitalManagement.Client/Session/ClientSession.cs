using HospitalManagement.Shared.DTOs;

namespace HospitalManagement.Client.Session;

/// <summary>
/// Holds the authenticated user's session data for the lifetime of the application.
/// Populated after a successful login and cleared on logout.
/// </summary>
public static class ClientSession
{
    /// <summary>JWT bearer token — sent in the Authorization header for every API request</summary>
    public static string? Token { get; private set; }

    public static string? UserId { get; private set; }
    public static string? Username { get; private set; }
    public static string? FullName { get; private set; }

    /// <summary>Role: Admin | Doctor | Nurse | Patient</summary>
    public static string? Role { get; private set; }

    public static DateTime? ExpiresAt { get; private set; }

    /// <summary>Returns true when a valid, non-expired token is present</summary>
    public static bool IsAuthenticated =>
        Token is not null && ExpiresAt.HasValue && DateTime.UtcNow < ExpiresAt.Value;

    /// <summary>Stores the auth response received after a successful login or register</summary>
    public static void SetSession(AuthResponse response)
    {
        Token = response.Token;
        UserId = response.UserId;
        Username = response.Username;
        FullName = response.FullName;
        Role = response.Role;
        ExpiresAt = response.ExpiresAt;
    }

    /// <summary>Clears all session data (called on logout)</summary>
    public static void Clear()
    {
        Token = null;
        UserId = null;
        Username = null;
        FullName = null;
        Role = null;
        ExpiresAt = null;
    }

    // --- Role helpers ---
    public static bool IsAdmin => Role == "Admin";
    public static bool IsDoctor => Role == "Doctor";
    public static bool IsNurse => Role == "Nurse";
    public static bool IsPatient => Role == "Patient";
    public static bool IsStaff => IsAdmin || IsDoctor || IsNurse;
}
