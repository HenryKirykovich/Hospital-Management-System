namespace HospitalManagement.Server.Settings;

/// <summary>
/// Strongly-typed settings bound from appsettings.json "MongoDb" section.
/// Injected via IOptions&lt;MongoDbSettings&gt;.
/// </summary>
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "Hospital-Management-System";

    // Collection names — centralised to avoid magic strings across the codebase
    public string UsersCollection { get; set; } = "users";
    public string PatientsCollection { get; set; } = "patients";
    public string AppointmentsCollection { get; set; } = "appointments";
    public string InventoryCollection { get; set; } = "inventory";
    public string ChatMessagesCollection { get; set; } = "chat_messages";
    public string VitalsCollection { get; set; } = "vitals";
}
