using HospitalManagement.Shared.Models;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Contract for patient CRUD operations and medical history management.
/// </summary>
public interface IPatientService
{
    Task<List<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(string id);
    Task<List<Patient>> SearchAsync(string query);
    Task<Patient> CreateAsync(Patient patient);
    Task<bool> UpdateAsync(string id, Patient patient);
    Task<bool> DeleteAsync(string id);
    Task<bool> AddMedicalRecordAsync(string patientId, MedicalRecord record);
}
