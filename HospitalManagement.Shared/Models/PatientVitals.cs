using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HospitalManagement.Shared.Models;

/// <summary>
/// Represents real-time vital signs for a patient in critical care.
/// Pushed via SignalR to the monitoring dashboard.
/// </summary>
public class PatientVitals
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("patientId")]
    public string PatientId { get; set; } = string.Empty;

    [BsonElement("patientName")]
    public string PatientName { get; set; } = string.Empty;

    [BsonElement("roomNumber")]
    public string RoomNumber { get; set; } = string.Empty;

    [BsonElement("heartRate")]
    public double HeartRate { get; set; }

    [BsonElement("bloodPressureSystolic")]
    public double BloodPressureSystolic { get; set; }

    [BsonElement("bloodPressureDiastolic")]
    public double BloodPressureDiastolic { get; set; }

    [BsonElement("temperature")]
    public double Temperature { get; set; }

    [BsonElement("oxygenSaturation")]
    public double OxygenSaturation { get; set; }

    [BsonElement("recordedAt")]
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Alert flag set by server if any vital is outside normal range</summary>
    [BsonElement("isCritical")]
    public bool IsCritical { get; set; } = false;
}
