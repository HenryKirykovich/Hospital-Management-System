using HospitalManagement.Server.Data;
using HospitalManagement.Shared.Models;
using MongoDB.Driver;

namespace HospitalManagement.Server.Seeds;

/// <summary>
/// Seeds the database with realistic fake data on first startup (Development only).
/// Runs only when the users collection is empty — safe to leave enabled.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(MongoDbContext db)
    {
        // Only seed if database is empty
        var count = await db.Users.CountDocumentsAsync(_ => true);
        if (count > 0) return;

        Console.WriteLine("Seeding database with sample data...");

        // --- Users (Doctors, Nurses, Admin, Patients) ---
        var users = BuildUsers();
        await db.Users.InsertManyAsync(users);

        var doctors = users.Where(u => u.Role == "Doctor").ToList();
        var patientUsers = users.Where(u => u.Role == "Patient").ToList();

        // --- Patients ---
        var patients = BuildPatients(patientUsers, doctors);
        await db.Patients.InsertManyAsync(patients);

        // --- Appointments ---
        var appointments = BuildAppointments(patients, doctors);
        await db.Appointments.InsertManyAsync(appointments);

        // --- Inventory ---
        var inventory = BuildInventory();
        await db.Inventory.InsertManyAsync(inventory);

        Console.WriteLine($"Seeded: {users.Count} users, {patients.Count} patients, " +
                          $"{appointments.Count} appointments, {inventory.Count} inventory items.");
        Console.WriteLine("Default password for all seeded accounts: Password123!");
    }

    // -------------------------------------------------------------------------
    // Users
    // -------------------------------------------------------------------------
    private static List<User> BuildUsers()
    {
        var hash = (string pw) => BCrypt.Net.BCrypt.HashPassword(pw, workFactor: 12);
        const string defaultPassword = "Password123!";

        return new List<User>
        {
            // Admin
            new() { Username = "admin", Email = "admin@hospital.com",
                    PasswordHash = hash(defaultPassword), Role = "Admin",
                    FullName = "System Administrator", IsActive = true },

            // Doctors
            new() { Username = "dr.smith", Email = "j.smith@hospital.com",
                    PasswordHash = hash(defaultPassword), Role = "Doctor",
                    FullName = "Dr. James Smith", IsActive = true },
            new() { Username = "dr.johnson", Email = "s.johnson@hospital.com",
                    PasswordHash = hash(defaultPassword), Role = "Doctor",
                    FullName = "Dr. Sarah Johnson", IsActive = true },
            new() { Username = "dr.williams", Email = "r.williams@hospital.com",
                    PasswordHash = hash(defaultPassword), Role = "Doctor",
                    FullName = "Dr. Robert Williams", IsActive = true },
            new() { Username = "dr.brown", Email = "e.brown@hospital.com",
                    PasswordHash = hash(defaultPassword), Role = "Doctor",
                    FullName = "Dr. Emily Brown", IsActive = true },

            // Nurses
            new() { Username = "nurse.davis", Email = "m.davis@hospital.com",
                    PasswordHash = hash(defaultPassword), Role = "Nurse",
                    FullName = "Maria Davis", IsActive = true },
            new() { Username = "nurse.wilson", Email = "t.wilson@hospital.com",
                    PasswordHash = hash(defaultPassword), Role = "Nurse",
                    FullName = "Thomas Wilson", IsActive = true },
            new() { Username = "nurse.moore", Email = "l.moore@hospital.com",
                    PasswordHash = hash(defaultPassword), Role = "Nurse",
                    FullName = "Laura Moore", IsActive = true },

            // Patients
            new() { Username = "patient.jones", Email = "d.jones@email.com",
                    PasswordHash = hash(defaultPassword), Role = "Patient",
                    FullName = "David Jones", IsActive = true },
            new() { Username = "patient.taylor", Email = "a.taylor@email.com",
                    PasswordHash = hash(defaultPassword), Role = "Patient",
                    FullName = "Anna Taylor", IsActive = true },
            new() { Username = "patient.anderson", Email = "m.anderson@email.com",
                    PasswordHash = hash(defaultPassword), Role = "Patient",
                    FullName = "Michael Anderson", IsActive = true },
            new() { Username = "patient.thomas", Email = "j.thomas@email.com",
                    PasswordHash = hash(defaultPassword), Role = "Patient",
                    FullName = "Jennifer Thomas", IsActive = true },
            new() { Username = "patient.jackson", Email = "c.jackson@email.com",
                    PasswordHash = hash(defaultPassword), Role = "Patient",
                    FullName = "Charles Jackson", IsActive = true },
        };
    }

    // -------------------------------------------------------------------------
    // Patients
    // -------------------------------------------------------------------------
    private static List<Patient> BuildPatients(List<User> patientUsers, List<User> doctors)
    {
        var doctorId = doctors.First().Id!;

        return new List<Patient>
        {
            new() {
                UserId = patientUsers[0].Id,
                FirstName = "David", LastName = "Jones",
                DateOfBirth = new DateTime(1985, 3, 22),
                Gender = "Male", Phone = "555-0101",
                Email = "d.jones@email.com", Address = "12 Oak Street, Springfield",
                BloodType = "A+", Allergies = new() { "Penicillin" },
                MedicalHistory = new()
                {
                    new() { Date = DateTime.UtcNow.AddMonths(-6), Diagnosis = "Hypertension",
                            Treatment = "Lisinopril 10mg daily", DoctorId = doctorId,
                            Notes = "Blood pressure well controlled" },
                    new() { Date = DateTime.UtcNow.AddMonths(-2), Diagnosis = "Annual checkup",
                            Treatment = "No treatment required", DoctorId = doctorId, Notes = "All clear" }
                }
            },
            new() {
                UserId = patientUsers[1].Id,
                FirstName = "Anna", LastName = "Taylor",
                DateOfBirth = new DateTime(1992, 7, 14),
                Gender = "Female", Phone = "555-0102",
                Email = "a.taylor@email.com", Address = "45 Maple Ave, Shelbyville",
                BloodType = "B+", Allergies = new() { "Sulfa drugs", "Latex" },
                MedicalHistory = new()
                {
                    new() { Date = DateTime.UtcNow.AddMonths(-3), Diagnosis = "Type 2 Diabetes",
                            Treatment = "Metformin 500mg twice daily", DoctorId = doctorId,
                            Notes = "HbA1c 7.2%" }
                }
            },
            new() {
                UserId = patientUsers[2].Id,
                FirstName = "Michael", LastName = "Anderson",
                DateOfBirth = new DateTime(1978, 11, 5),
                Gender = "Male", Phone = "555-0103",
                Email = "m.anderson@email.com", Address = "78 Elm Road, Capital City",
                BloodType = "O-", Allergies = new(),
                MedicalHistory = new()
                {
                    new() { Date = DateTime.UtcNow.AddMonths(-1), Diagnosis = "Lower back pain",
                            Treatment = "Physical therapy, Ibuprofen 400mg as needed",
                            DoctorId = doctorId, Notes = "Referred to physiotherapy" }
                }
            },
            new() {
                UserId = patientUsers[3].Id,
                FirstName = "Jennifer", LastName = "Thomas",
                DateOfBirth = new DateTime(2000, 4, 18),
                Gender = "Female", Phone = "555-0104",
                Email = "j.thomas@email.com", Address = "33 Pine Lane, Shelbyville",
                BloodType = "AB+", Allergies = new() { "Aspirin" },
                MedicalHistory = new()
            },
            new() {
                UserId = patientUsers[4].Id,
                FirstName = "Charles", LastName = "Jackson",
                DateOfBirth = new DateTime(1965, 9, 30),
                Gender = "Male", Phone = "555-0105",
                Email = "c.jackson@email.com", Address = "90 Birch Blvd, Springfield",
                BloodType = "A-", Allergies = new() { "Codeine", "NSAIDs" },
                MedicalHistory = new()
                {
                    new() { Date = DateTime.UtcNow.AddMonths(-8), Diagnosis = "Coronary artery disease",
                            Treatment = "Atorvastatin 40mg, Aspirin 81mg daily",
                            DoctorId = doctorId, Notes = "Follow-up in 3 months" },
                    new() { Date = DateTime.UtcNow.AddMonths(-4), Diagnosis = "Follow-up visit",
                            Treatment = "Continue current medications", DoctorId = doctorId,
                            Notes = "Cholesterol improved" }
                }
            },
            // Additional patients without user accounts
            new() {
                FirstName = "Sophie", LastName = "Martinez",
                DateOfBirth = new DateTime(1990, 6, 10),
                Gender = "Female", Phone = "555-0201",
                Email = "s.martinez@email.com", Address = "21 Cedar St, Springfield",
                BloodType = "O+", Allergies = new(),
                MedicalHistory = new()
            },
            new() {
                FirstName = "William", LastName = "Clark",
                DateOfBirth = new DateTime(1955, 12, 3),
                Gender = "Male", Phone = "555-0202",
                Email = "w.clark@email.com", Address = "56 Walnut Drive, Capital City",
                BloodType = "B-", Allergies = new() { "Iodine contrast" },
                MedicalHistory = new()
                {
                    new() { Date = DateTime.UtcNow.AddMonths(-5), Diagnosis = "Osteoarthritis",
                            Treatment = "Celecoxib 200mg daily", DoctorId = doctorId,
                            Notes = "Knee X-ray ordered" }
                }
            },
            new() {
                FirstName = "Olivia", LastName = "Rodriguez",
                DateOfBirth = new DateTime(2005, 2, 27),
                Gender = "Female", Phone = "555-0203",
                Email = "o.rodriguez@email.com", Address = "14 Spruce Ave, Shelbyville",
                BloodType = "A+", Allergies = new(),
                MedicalHistory = new()
            },
        };
    }

    // -------------------------------------------------------------------------
    // Appointments
    // -------------------------------------------------------------------------
    private static List<Appointment> BuildAppointments(List<Patient> patients, List<User> doctors)
    {
        var appointments = new List<Appointment>();
        var statuses = new[] { "Scheduled", "Confirmed", "Confirmed", "Completed", "Completed" };
        var reasons = new[]
        {
            "Annual checkup", "Follow-up visit", "Blood pressure monitoring",
            "Diabetes management", "Back pain consultation",
            "Cardiology review", "Prescription renewal", "Lab results review"
        };

        var rng = new Random(42);

        for (int i = 0; i < patients.Count; i++)
        {
            var doctor = doctors[i % doctors.Count];
            var patient = patients[i];

            // Past appointment (completed)
            appointments.Add(new Appointment
            {
                PatientId = patient.Id ?? string.Empty,
                PatientName = $"{patient.FirstName} {patient.LastName}",
                DoctorId = doctor.Id ?? string.Empty,
                DoctorName = doctor.FullName,
                ScheduledAt = DateTime.UtcNow.AddDays(-(rng.Next(5, 30))),
                DurationMinutes = 30,
                Reason = reasons[rng.Next(reasons.Length)],
                Status = "Completed",
                Notes = "Patient seen, no complications."
            });

            // Upcoming appointment
            appointments.Add(new Appointment
            {
                PatientId = patient.Id ?? string.Empty,
                PatientName = $"{patient.FirstName} {patient.LastName}",
                DoctorId = doctors[(i + 1) % doctors.Count].Id ?? string.Empty,
                DoctorName = doctors[(i + 1) % doctors.Count].FullName,
                ScheduledAt = DateTime.UtcNow.AddDays(rng.Next(1, 14)).AddHours(rng.Next(8, 17)),
                DurationMinutes = rng.Next(1, 3) * 15 + 15,
                Reason = reasons[rng.Next(reasons.Length)],
                Status = statuses[rng.Next(statuses.Length - 2)], // Only Scheduled/Confirmed for future
                Notes = string.Empty
            });
        }

        return appointments;
    }

    // -------------------------------------------------------------------------
    // Inventory
    // -------------------------------------------------------------------------
    private static List<InventoryItem> BuildInventory()
    {
        return new List<InventoryItem>
        {
            new() { Name = "Paracetamol 500mg", Category = "Medication",
                    Description = "Pain relief and fever reducer",
                    Quantity = 500, Unit = "tablets", LowStockThreshold = 50,
                    UnitPrice = 0.10m, Supplier = "PharmaCo" },
            new() { Name = "Amoxicillin 250mg", Category = "Medication",
                    Description = "Broad-spectrum antibiotic",
                    Quantity = 200, Unit = "capsules", LowStockThreshold = 30,
                    UnitPrice = 0.45m, Supplier = "MediSupply" },
            new() { Name = "Ibuprofen 400mg", Category = "Medication",
                    Description = "NSAID anti-inflammatory",
                    Quantity = 8, Unit = "tablets", LowStockThreshold = 50,
                    UnitPrice = 0.12m, Supplier = "PharmaCo" },  // LOW STOCK
            new() { Name = "Lisinopril 10mg", Category = "Medication",
                    Description = "ACE inhibitor for hypertension",
                    Quantity = 150, Unit = "tablets", LowStockThreshold = 20,
                    UnitPrice = 0.30m, Supplier = "CardioMed" },
            new() { Name = "Metformin 500mg", Category = "Medication",
                    Description = "Type 2 diabetes management",
                    Quantity = 300, Unit = "tablets", LowStockThreshold = 40,
                    UnitPrice = 0.08m, Supplier = "DiabetesCare" },
            new() { Name = "Atorvastatin 40mg", Category = "Medication",
                    Description = "Cholesterol-lowering statin",
                    Quantity = 12, Unit = "tablets", LowStockThreshold = 25,
                    UnitPrice = 0.55m, Supplier = "CardioMed" },  // LOW STOCK
            new() { Name = "Surgical Gloves (M)", Category = "Supply",
                    Description = "Sterile latex-free surgical gloves",
                    Quantity = 500, Unit = "pairs", LowStockThreshold = 50,
                    UnitPrice = 0.80m, Supplier = "SurgicalPlus" },
            new() { Name = "Disposable Syringes 5ml", Category = "Supply",
                    Description = "Single-use syringes",
                    Quantity = 1000, Unit = "units", LowStockThreshold = 100,
                    UnitPrice = 0.15m, Supplier = "MedEquip" },
            new() { Name = "Blood Pressure Monitor", Category = "Equipment",
                    Description = "Digital automatic BP monitor",
                    Quantity = 5, Unit = "units", LowStockThreshold = 2,
                    UnitPrice = 45.00m, Supplier = "MedEquip" },
            new() { Name = "IV Saline Solution 500ml", Category = "Supply",
                    Description = "Normal saline 0.9%",
                    Quantity = 3, Unit = "bags", LowStockThreshold = 20,
                    UnitPrice = 2.50m, Supplier = "MediSupply" },  // LOW STOCK
            new() { Name = "Bandages 10cm", Category = "Supply",
                    Description = "Sterile gauze bandages",
                    Quantity = 200, Unit = "rolls", LowStockThreshold = 30,
                    UnitPrice = 0.60m, Supplier = "SurgicalPlus" },
            new() { Name = "Pulse Oximeter", Category = "Equipment",
                    Description = "Finger pulse oximeter for SpO2",
                    Quantity = 8, Unit = "units", LowStockThreshold = 3,
                    UnitPrice = 18.00m, Supplier = "MedEquip" },
        };
    }
}
