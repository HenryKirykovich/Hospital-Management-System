using HospitalManagement.Shared.Models;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Contract for patient vitals recording and monitoring.
/// </summary>
public interface IVitalsService
{
    /// <summary>Records a new vitals entry and flags it as critical if needed</summary>
    Task<PatientVitals> RecordAsync(PatientVitals vitals);

    /// <summary>Returns the latest vitals for each monitored patient (one per patient)</summary>
    Task<List<PatientVitals>> GetLatestAllAsync();

    /// <summary>Returns vitals history for a specific patient</summary>
    Task<List<PatientVitals>> GetPatientHistoryAsync(string patientId, int limit = 20);

    /// <summary>Returns all entries currently flagged as critical</summary>
    Task<List<PatientVitals>> GetCriticalAsync();
}
