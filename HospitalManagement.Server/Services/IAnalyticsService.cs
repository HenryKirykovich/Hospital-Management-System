using HospitalManagement.Shared.DTOs;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Provides aggregated statistics for the analytics dashboard.
/// </summary>
public interface IAnalyticsService
{
    Task<DashboardData> GetDashboardDataAsync();
}
