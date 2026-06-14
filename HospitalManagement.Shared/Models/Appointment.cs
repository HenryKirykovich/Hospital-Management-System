using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HospitalManagement.Shared.Models;

/// <summary>
/// Represents a scheduled appointment between a patient and a doctor.
/// Status changes trigger SignalR real-time notifications.
/// </summary>
public class Appointment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("patientId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string PatientId { get; set; } = string.Empty;

    [BsonElement("doctorId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string DoctorId { get; set; } = string.Empty;

    /// <summary>Denormalized names for quick display without extra lookups</summary>
    [BsonElement("patientName")]
    public string PatientName { get; set; } = string.Empty;

    [BsonElement("doctorName")]
    public string DoctorName { get; set; } = string.Empty;

    [BsonElement("scheduledAt")]
    public DateTime ScheduledAt { get; set; }

    [BsonElement("durationMinutes")]
    public int DurationMinutes { get; set; } = 30;

    [BsonElement("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>Status: Scheduled | Confirmed | Cancelled | Completed | NoShow</summary>
    [BsonElement("status")]
    public string Status { get; set; } = "Scheduled";

    [BsonElement("notes")]
    public string Notes { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
