using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HospitalManagement.Shared.Models;

/// <summary>
/// Represents a system user with role-based access control.
/// Roles: Admin, Doctor, Nurse, Patient
/// </summary>
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("username")]
    public string Username { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>BCrypt hashed password — never store plain text</summary>
    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Role: Admin | Doctor | Nurse | Patient</summary>
    [BsonElement("role")]
    public string Role { get; set; } = "Patient";

    [BsonElement("fullName")]
    public string FullName { get; set; } = string.Empty;

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("lastLogin")]
    public DateTime? LastLogin { get; set; }
}
