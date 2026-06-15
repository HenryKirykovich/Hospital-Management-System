using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HospitalManagement.Shared.Models;

/// <summary>
/// Represents a patient in the hospital system.
/// Linked to a User account via UserId (optional — patients can exist without a login).
/// </summary>
public class Patient
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>Optional reference to User.Id if the patient has a login account</summary>
    [BsonElement("userId")]
    public string? UserId { get; set; }

    [BsonElement("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("lastName")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("dateOfBirth")]
    public DateTime DateOfBirth { get; set; }

    [BsonElement("gender")]
    public string Gender { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    [BsonElement("bloodType")]
    public string BloodType { get; set; } = string.Empty;

    [BsonElement("allergies")]
    public List<string> Allergies { get; set; } = new();

    [BsonElement("medicalHistory")]
    public List<MedicalRecord> MedicalHistory { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    /// <summary>Computed display name for UI dropdowns — not stored in MongoDB</summary>
    [BsonIgnore]
    public string FullNameDisplay => $"{FirstName} {LastName}";
}

/// <summary>
/// Embedded document: a single entry in a patient's medical history
/// </summary>
public class MedicalRecord
{
    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("diagnosis")]
    public string Diagnosis { get; set; } = string.Empty;

    [BsonElement("treatment")]
    public string Treatment { get; set; } = string.Empty;

    [BsonElement("doctorId")]
    public string? DoctorId { get; set; }

    [BsonElement("notes")]
    public string Notes { get; set; } = string.Empty;
}
