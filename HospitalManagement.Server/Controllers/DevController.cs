using HospitalManagement.Server.Data;
using HospitalManagement.Server.Seeds;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace HospitalManagement.Server.Controllers;

/// <summary>
/// Development-only controller for seeding and resetting the database.
/// NOT available in Production.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DevController : ControllerBase
{
    private readonly MongoDbContext _db;
    private readonly IWebHostEnvironment _env;

    public DevController(MongoDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    /// <summary>
    /// POST /api/dev/reseed — drops all collections and reseeds with sample data.
    /// Only works in Development environment.
    /// </summary>
    [HttpPost("reseed")]
    public async Task<IActionResult> Reseed()
    {
        if (!_env.IsDevelopment())
            return Forbid();

        // Drop all collections
        await _db.Users.DeleteManyAsync(_ => true);
        await _db.Patients.DeleteManyAsync(_ => true);
        await _db.Appointments.DeleteManyAsync(_ => true);
        await _db.Inventory.DeleteManyAsync(_ => true);
        await _db.ChatMessages.DeleteManyAsync(_ => true);
        await _db.Vitals.DeleteManyAsync(_ => true);

        // Reseed
        await DataSeeder.SeedAsync(_db);

        return Ok(new
        {
            message = "Database reseeded successfully.",
            note = "Default password for all accounts: Password123!"
        });
    }
}
