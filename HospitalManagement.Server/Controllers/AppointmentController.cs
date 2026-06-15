using HospitalManagement.Server.Hubs;
using HospitalManagement.Server.Services;
using HospitalManagement.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HospitalManagement.Server.Controllers;

/// <summary>
/// REST API for appointment management.
/// Status changes broadcast real-time notifications via SignalR to all connected clients.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly IHubContext<HospitalHub> _hub;
    private readonly ILogger<AppointmentController> _logger;

    public AppointmentController(
        IAppointmentService appointmentService,
        IHubContext<HospitalHub> hub,
        ILogger<AppointmentController> logger)
    {
        _appointmentService = appointmentService;
        _hub = hub;
        _logger = logger;
    }

    /// <summary>GET /api/appointment — all appointments sorted by date</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _appointmentService.GetAllAsync();
        return Ok(list);
    }

    /// <summary>GET /api/appointment/{id}</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var a = await _appointmentService.GetByIdAsync(id);
        if (a is null) return NotFound(new { message = $"Appointment '{id}' not found." });
        return Ok(a);
    }

    /// <summary>GET /api/appointment/doctor/{doctorId}</summary>
    [HttpGet("doctor/{doctorId}")]
    public async Task<IActionResult> GetByDoctor(string doctorId)
    {
        var list = await _appointmentService.GetByDoctorAsync(doctorId);
        return Ok(list);
    }

    /// <summary>GET /api/appointment/patient/{patientId}</summary>
    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetByPatient(string patientId)
    {
        var list = await _appointmentService.GetByPatientAsync(patientId);
        return Ok(list);
    }

    /// <summary>GET /api/appointment/range?from=...&to=...</summary>
    [HttpGet("range")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var list = await _appointmentService.GetByDateRangeAsync(from, to);
        return Ok(list);
    }

    /// <summary>POST /api/appointment — creates a new appointment and notifies all clients</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> Create([FromBody] Appointment appointment)
    {
        var created = await _appointmentService.CreateAsync(appointment);

        // Broadcast new appointment to all connected clients
        await _hub.Clients.All.SendAsync("AppointmentCreated", created);

        _logger.LogInformation("Appointment created for patient {PatientId} with doctor {DoctorId}",
            appointment.PatientId, appointment.DoctorId);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>PUT /api/appointment/{id} — updates appointment details and notifies clients</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> Update(string id, [FromBody] Appointment appointment)
    {
        var updated = await _appointmentService.UpdateAsync(id, appointment);
        if (!updated) return NotFound(new { message = $"Appointment '{id}' not found." });

        appointment.Id = id;
        await _hub.Clients.All.SendAsync("AppointmentUpdated", appointment);

        return NoContent();
    }

    /// <summary>
    /// PATCH /api/appointment/{id}/status — changes status only (Confirmed/Cancelled/Completed/NoShow).
    /// Triggers a targeted SignalR notification.
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] StatusUpdateRequest request)
    {
        var updated = await _appointmentService.UpdateStatusAsync(id, request.Status);
        if (!updated) return NotFound(new { message = $"Appointment '{id}' not found." });

        // Broadcast status change to all clients with appointment id and new status
        await _hub.Clients.All.SendAsync("AppointmentStatusChanged", new
        {
            appointmentId = id,
            status = request.Status,
            updatedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Appointment {Id} status changed to {Status}", id, request.Status);
        return NoContent();
    }

    /// <summary>DELETE /api/appointment/{id} — removes appointment and notifies clients</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _appointmentService.DeleteAsync(id);
        if (!deleted) return NotFound(new { message = $"Appointment '{id}' not found." });

        await _hub.Clients.All.SendAsync("AppointmentDeleted", id);

        return NoContent();
    }
}

/// <summary>Payload for PATCH status endpoint</summary>
public record StatusUpdateRequest(string Status);
