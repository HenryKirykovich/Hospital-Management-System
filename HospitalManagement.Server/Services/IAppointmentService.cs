using HospitalManagement.Shared.Models;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Contract for appointment CRUD and status management.
/// </summary>
public interface IAppointmentService
{
    Task<List<Appointment>> GetAllAsync();
    Task<List<Appointment>> GetByDoctorAsync(string doctorId);
    Task<List<Appointment>> GetByPatientAsync(string patientId);
    Task<List<Appointment>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<Appointment?> GetByIdAsync(string id);
    Task<Appointment> CreateAsync(Appointment appointment);
    Task<bool> UpdateAsync(string id, Appointment appointment);
    Task<bool> UpdateStatusAsync(string id, string status);
    Task<bool> DeleteAsync(string id);
}
