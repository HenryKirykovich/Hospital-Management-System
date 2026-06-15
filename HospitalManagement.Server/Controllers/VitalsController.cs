using HospitalManagement.Server.Hubs;
using HospitalManagement.Server.Services;
using HospitalManagement.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HospitalManagement.Server.Controllers;

/// <summary>
/// REST API for patient vitals recording and monitoring.
/// Every POST broadcasts the new record via SignalR.
/// Critical vitals also send a Notification to all staff.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VitalsController : ControllerBase
{
    private readonly IVitalsService _vitalsService;
    private readonly IHubContext<HospitalHub> _hub;
    private readonly ILogger<VitalsController> _logger;

    public VitalsController(IVitalsService vitalsService, IHubContext<HospitalHub> hub, ILogger<VitalsController> logger)
    {
        _vitalsService = vitalsService;
        _hub = hub;
        _logger = logger;
    }

    /// <summary>GET /api/vitals — latest vitals for all monitored patients</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> GetLatestAll()
    {
        var vitals = await _vitalsService.GetLatestAllAsync();
        return Ok(vitals);
    }

    /// <summary>GET /api/vitals/critical — all currently critical entries</summary>
    [HttpGet("critical")]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> GetCritical()
    {
        var critical = await _vitalsService.GetCriticalAsync();
        return Ok(critical);
    }

    /// <summary>GET /api/vitals/patient/{id} — vitals history for one patient</summary>
    [HttpGet("patient/{patientId}")]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> GetPatientHistory(string patientId, [FromQuery] int limit = 20)
    {
        var history = await _vitalsService.GetPatientHistoryAsync(patientId, limit);
        return Ok(history);
    }

    /// <summary>
    /// POST /api/vitals — records new vitals for a patient.
    /// Broadcasts VitalsUpdate to all clients.
    /// If critical, also broadcasts a Notification to staff.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> Record([FromBody] PatientVitals vitals)
    {
        if (string.IsNullOrWhiteSpace(vitals.PatientId))
            return BadRequest(new { message = "PatientId is required." });

        var saved = await _vitalsService.RecordAsync(vitals);

        // Broadcast to all monitoring screens
        await _hub.Clients.All.SendAsync("VitalsUpdate", saved);

        if (saved.IsCritical)
        {
            var alert = $"🚨 CRITICAL: {saved.PatientName} (Room {saved.RoomNumber}) — HR:{saved.HeartRate} BP:{saved.BloodPressureSystolic}/{saved.BloodPressureDiastolic} O₂:{saved.OxygenSaturation}%";
            await _hub.Clients.Groups("Admin", "Doctor", "Nurse")
                .SendAsync("Notification", alert);
            _logger.LogWarning("Critical vitals for patient {Name}: {Alert}", saved.PatientName, alert);
        }

        return CreatedAtAction(nameof(GetPatientHistory), new { patientId = saved.PatientId }, saved);
    }
}
