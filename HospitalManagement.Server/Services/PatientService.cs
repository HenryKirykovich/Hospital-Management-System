using HospitalManagement.Server.Data;
using HospitalManagement.Shared.Models;
using MongoDB.Driver;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Handles all patient CRUD operations and medical history entries against MongoDB.
/// </summary>
public class PatientService : IPatientService
{
    private readonly MongoDbContext _db;

    public PatientService(MongoDbContext db)
    {
        _db = db;
    }

    /// <summary>Returns all active patients, sorted by last name</summary>
    public async Task<List<Patient>> GetAllAsync()
    {
        return await _db.Patients
            .Find(p => p.IsActive)
            .SortBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    /// <summary>Returns a single patient by MongoDB ObjectId</summary>
    public async Task<Patient?> GetByIdAsync(string id)
    {
        return await _db.Patients
            .Find(p => p.Id == id && p.IsActive)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Full-text search across first name, last name, email, and phone.
    /// Case-insensitive regex match.
    /// </summary>
    public async Task<List<Patient>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetAllAsync();

        var filter = Builders<Patient>.Filter.And(
            Builders<Patient>.Filter.Eq(p => p.IsActive, true),
            Builders<Patient>.Filter.Or(
                Builders<Patient>.Filter.Regex(p => p.FirstName,
                    new MongoDB.Bson.BsonRegularExpression(query, "i")),
                Builders<Patient>.Filter.Regex(p => p.LastName,
                    new MongoDB.Bson.BsonRegularExpression(query, "i")),
                Builders<Patient>.Filter.Regex(p => p.Email,
                    new MongoDB.Bson.BsonRegularExpression(query, "i")),
                Builders<Patient>.Filter.Regex(p => p.Phone,
                    new MongoDB.Bson.BsonRegularExpression(query, "i"))
            ));

        return await _db.Patients
            .Find(filter)
            .SortBy(p => p.LastName)
            .ToListAsync();
    }

    /// <summary>Inserts a new patient record</summary>
    public async Task<Patient> CreateAsync(Patient patient)
    {
        patient.CreatedAt = DateTime.UtcNow;
        patient.IsActive = true;
        await _db.Patients.InsertOneAsync(patient);
        return patient;
    }

    /// <summary>Updates all fields of an existing patient. Returns false if not found.</summary>
    public async Task<bool> UpdateAsync(string id, Patient patient)
    {
        patient.Id = id;
        var result = await _db.Patients.ReplaceOneAsync(
            p => p.Id == id && p.IsActive, patient);
        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// Soft-deletes a patient by setting IsActive = false.
    /// Data is retained for audit/history purposes.
    /// </summary>
    public async Task<bool> DeleteAsync(string id)
    {
        var update = Builders<Patient>.Update.Set(p => p.IsActive, false);
        var result = await _db.Patients.UpdateOneAsync(p => p.Id == id, update);
        return result.ModifiedCount > 0;
    }

    /// <summary>Appends a new medical record to the patient's history array</summary>
    public async Task<bool> AddMedicalRecordAsync(string patientId, MedicalRecord record)
    {
        record.Date = DateTime.UtcNow;
        var update = Builders<Patient>.Update.Push(p => p.MedicalHistory, record);
        var result = await _db.Patients.UpdateOneAsync(
            p => p.Id == patientId && p.IsActive, update);
        return result.ModifiedCount > 0;
    }
}
