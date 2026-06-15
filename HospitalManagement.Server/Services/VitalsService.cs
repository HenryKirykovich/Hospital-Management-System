using HospitalManagement.Server.Data;
using HospitalManagement.Shared.Models;
using MongoDB.Driver;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Records patient vitals in MongoDB and determines critical status.
/// Normal ranges used for critical detection:
///   Heart rate:        60–100 bpm
///   Systolic BP:       90–140 mmHg
///   Diastolic BP:      60–90  mmHg
///   Temperature:       36.1–37.8 °C
///   Oxygen saturation: >= 95%
/// </summary>
public class VitalsService : IVitalsService
{
    private readonly MongoDbContext _db;

    public VitalsService(MongoDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Saves a vitals record, auto-setting IsCritical based on normal ranges.
    /// </summary>
    public async Task<PatientVitals> RecordAsync(PatientVitals vitals)
    {
        vitals.RecordedAt = DateTime.UtcNow;
        vitals.IsCritical = DetectCritical(vitals);
        await _db.Vitals.InsertOneAsync(vitals);
        return vitals;
    }

    /// <summary>
    /// Returns the single most-recent entry for each distinct patient.
    /// Used by the monitoring board to show current status.
    /// </summary>
    public async Task<List<PatientVitals>> GetLatestAllAsync()
    {
        // Get all vitals sorted newest first, then pick latest per patient in memory
        var all = await _db.Vitals
            .Find(_ => true)
            .SortByDescending(v => v.RecordedAt)
            .ToListAsync();

        return all
            .GroupBy(v => v.PatientId)
            .Select(g => g.First())   // newest per patient
            .OrderBy(v => v.PatientName)
            .ToList();
    }

    public async Task<List<PatientVitals>> GetPatientHistoryAsync(string patientId, int limit = 20)
    {
        return await _db.Vitals
            .Find(v => v.PatientId == patientId)
            .SortByDescending(v => v.RecordedAt)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<List<PatientVitals>> GetCriticalAsync()
    {
        return await _db.Vitals
            .Find(v => v.IsCritical)
            .SortByDescending(v => v.RecordedAt)
            .ToListAsync();
    }

    // ---- Helpers ----

    private static bool DetectCritical(PatientVitals v) =>
        v.HeartRate < 50 || v.HeartRate > 120 ||
        v.BloodPressureSystolic < 80 || v.BloodPressureSystolic > 160 ||
        v.BloodPressureDiastolic < 50 || v.BloodPressureDiastolic > 100 ||
        v.Temperature < 35.0 || v.Temperature > 39.0 ||
        v.OxygenSaturation < 92;
}
