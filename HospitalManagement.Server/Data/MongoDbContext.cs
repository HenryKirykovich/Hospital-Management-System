using HospitalManagement.Server.Settings;
using HospitalManagement.Shared.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HospitalManagement.Server.Data;

/// <summary>
/// Provides typed access to all MongoDB collections.
/// Registered as a singleton — MongoClient is thread-safe by design.
/// </summary>
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);

        EnsureIndexes(settings.Value);
    }

    // --- Collections ---
    public IMongoCollection<User> Users =>
        _database.GetCollection<User>("users");

    public IMongoCollection<Patient> Patients =>
        _database.GetCollection<Patient>("patients");

    public IMongoCollection<Appointment> Appointments =>
        _database.GetCollection<Appointment>("appointments");

    public IMongoCollection<InventoryItem> Inventory =>
        _database.GetCollection<InventoryItem>("inventory");

    public IMongoCollection<ChatMessage> ChatMessages =>
        _database.GetCollection<ChatMessage>("chat_messages");

    public IMongoCollection<PatientVitals> Vitals =>
        _database.GetCollection<PatientVitals>("vitals");

    /// <summary>
    /// Creates indexes on first startup to ensure query performance.
    /// Idempotent — safe to call every time the app starts.
    /// </summary>
    private void EnsureIndexes(MongoDbSettings settings)
    {
        // Unique index on username and email for users
        Users.Indexes.CreateOne(new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Username),
            new CreateIndexOptions { Unique = true, Name = "idx_users_username" }));

        Users.Indexes.CreateOne(new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true, Name = "idx_users_email" }));

        // Index for appointment queries by doctor and patient
        Appointments.Indexes.CreateOne(new CreateIndexModel<Appointment>(
            Builders<Appointment>.IndexKeys
                .Ascending(a => a.DoctorId)
                .Ascending(a => a.ScheduledAt),
            new CreateIndexOptions { Name = "idx_appointments_doctor_date" }));

        Appointments.Indexes.CreateOne(new CreateIndexModel<Appointment>(
            Builders<Appointment>.IndexKeys.Ascending(a => a.PatientId),
            new CreateIndexOptions { Name = "idx_appointments_patient" }));

        // Index for chat message queries by channel
        ChatMessages.Indexes.CreateOne(new CreateIndexModel<ChatMessage>(
            Builders<ChatMessage>.IndexKeys
                .Ascending(m => m.Channel)
                .Descending(m => m.SentAt),
            new CreateIndexOptions { Name = "idx_chat_channel_date" }));

        // Index for vitals queries by patient
        Vitals.Indexes.CreateOne(new CreateIndexModel<PatientVitals>(
            Builders<PatientVitals>.IndexKeys
                .Ascending(v => v.PatientId)
                .Descending(v => v.RecordedAt),
            new CreateIndexOptions { Name = "idx_vitals_patient_date" }));
    }
}
