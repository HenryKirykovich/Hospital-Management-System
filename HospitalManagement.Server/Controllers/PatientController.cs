using HospitalManagement.Server.Services;
using HospitalManagement.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Server.Controllers;

/// <summary>
/// REST API for patient management.
/// All endpoints require authentication.
/// Deletion is restricted to Admin and Doctor roles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientController> _logger;

    public PatientController(IPatientService patientService, ILogger<PatientController> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    /// <summary>GET /api/patient — returns all active patients</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _patientService.GetAllAsync();
        return Ok(patients);
    }

    /// <summary>GET /api/patient/{id} — returns a single patient by ID</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient is null)
            return NotFound(new { message = $"Patient '{id}' not found." });

        return Ok(patient);
    }

    /// <summary>GET /api/patient/search?q=... — searches patients by name, email, phone</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var results = await _patientService.SearchAsync(q);
        return Ok(results);
    }

    /// <summary>POST /api/patient — creates a new patient record</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> Create([FromBody] Patient patient)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _patientService.CreateAsync(patient);
        _logger.LogInformation("Patient created: {FirstName} {LastName}", patient.FirstName, patient.LastName);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>PUT /api/patient/{id} — updates an existing patient record</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> Update(string id, [FromBody] Patient patient)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _patientService.UpdateAsync(id, patient);
        if (!updated)
            return NotFound(new { message = $"Patient '{id}' not found." });

        _logger.LogInformation("Patient updated: {Id}", id);
        return NoContent();
    }

    /// <summary>DELETE /api/patient/{id} — soft-deletes a patient (Admin/Doctor only)</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _patientService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Patient '{id}' not found." });

        _logger.LogInformation("Patient soft-deleted: {Id}", id);
        return NoContent();
    }

    /// <summary>POST /api/patient/{id}/medical-record — appends a medical record to patient history</summary>
    [HttpPost("{id}/medical-record")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> AddMedicalRecord(string id, [FromBody] MedicalRecord record)
    {
        var added = await _patientService.AddMedicalRecordAsync(id, record);
        if (!added)
            return NotFound(new { message = $"Patient '{id}' not found." });

        _logger.LogInformation("Medical record added to patient: {Id}", id);
        return Ok(new { message = "Medical record added successfully." });
    }
}
