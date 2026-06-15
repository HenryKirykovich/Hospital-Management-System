using HospitalManagement.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Server.Controllers;

/// <summary>
/// Provides aggregated statistics for the analytics dashboard.
/// Only authenticated staff can access these endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Doctor,Nurse")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>GET /api/analytics/dashboard — all stats in one request</summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var data = await _analyticsService.GetDashboardDataAsync();
        return Ok(data);
    }
}
