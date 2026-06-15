using HospitalManagement.Server.Data;
using HospitalManagement.Shared.Models;
using MongoDB.Driver;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Handles all appointment CRUD operations against MongoDB.
/// Status changes are broadcast via SignalR in the controller layer.
/// </summary>
public class AppointmentService : IAppointmentService
{
    private readonly MongoDbContext _db;

    public AppointmentService(MongoDbContext db)
    {
        _db = db;
    }

    public async Task<List<Appointment>> GetAllAsync()
    {
        return await _db.Appointments
            .Find(_ => true)
            .SortBy(a => a.ScheduledAt)
            .ToListAsync();
    }

    public async Task<List<Appointment>> GetByDoctorAsync(string doctorId)
    {
        return await _db.Appointments
            .Find(a => a.DoctorId == doctorId)
            .SortBy(a => a.ScheduledAt)
            .ToListAsync();
    }

    public async Task<List<Appointment>> GetByPatientAsync(string patientId)
    {
        return await _db.Appointments
            .Find(a => a.PatientId == patientId)
            .SortBy(a => a.ScheduledAt)
            .ToListAsync();
    }

    public async Task<List<Appointment>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var filter = Builders<Appointment>.Filter.And(
            Builders<Appointment>.Filter.Gte(a => a.ScheduledAt, from),
            Builders<Appointment>.Filter.Lte(a => a.ScheduledAt, to));

        return await _db.Appointments
            .Find(filter)
            .SortBy(a => a.ScheduledAt)
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdAsync(string id)
    {
        return await _db.Appointments
            .Find(a => a.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        appointment.CreatedAt = DateTime.UtcNow;
        appointment.UpdatedAt = DateTime.UtcNow;
        appointment.Status = "Scheduled";
        await _db.Appointments.InsertOneAsync(appointment);
        return appointment;
    }

    public async Task<bool> UpdateAsync(string id, Appointment appointment)
    {
        appointment.Id = id;
        appointment.UpdatedAt = DateTime.UtcNow;
        var result = await _db.Appointments.ReplaceOneAsync(a => a.Id == id, appointment);
        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// Updates only the status field — called when appointment is confirmed/cancelled/completed.
    /// The controller broadcasts a SignalR event after this succeeds.
    /// </summary>
    public async Task<bool> UpdateStatusAsync(string id, string status)
    {
        var update = Builders<Appointment>.Update
            .Set(a => a.Status, status)
            .Set(a => a.UpdatedAt, DateTime.UtcNow);

        var result = await _db.Appointments.UpdateOneAsync(a => a.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _db.Appointments.DeleteOneAsync(a => a.Id == id);
        return result.DeletedCount > 0;
    }
}
