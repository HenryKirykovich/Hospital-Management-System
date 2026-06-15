using HospitalManagement.Server.Data;
using HospitalManagement.Shared.DTOs;
using MongoDB.Driver;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Runs MongoDB aggregation queries to produce dashboard statistics.
/// All queries execute in parallel where possible.
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly MongoDbContext _db;

    public AnalyticsService(MongoDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardData> GetDashboardDataAsync()
    {
        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var todayEnd = todayStart.AddDays(1);
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);
        var sixMonthsAgo = monthStart.AddMonths(-5);

        // Fire all queries in parallel
        var totalPatientsTask       = _db.Patients.CountDocumentsAsync(p => p.IsActive);
        var allPatientsTask         = _db.Patients.CountDocumentsAsync(_ => true);
        var staffTask               = _db.Users.CountDocumentsAsync(u => u.Role != "Patient" && u.IsActive);
        var apptTodayTask           = _db.Appointments.CountDocumentsAsync(a => a.ScheduledAt >= todayStart && a.ScheduledAt < todayEnd);
        var apptMonthTask           = _db.Appointments.CountDocumentsAsync(a => a.ScheduledAt >= monthStart && a.ScheduledAt < monthEnd);
        var apptPendingTask         = _db.Appointments.CountDocumentsAsync(a => a.Status == "Scheduled" || a.Status == "Confirmed");
        var allInventoryTask        = _db.Inventory.CountDocumentsAsync(_ => true);
        var allAppointmentsTask     = _db.Appointments.Find(_ => true).ToListAsync();
        var allInventoryItemsTask   = _db.Inventory.Find(_ => true).ToListAsync();

        await Task.WhenAll(
            totalPatientsTask, allPatientsTask, staffTask,
            apptTodayTask, apptMonthTask, apptPendingTask,
            allInventoryTask, allAppointmentsTask, allInventoryItemsTask);

        var allAppointments   = allAppointmentsTask.Result;
        var allInventoryItems = allInventoryItemsTask.Result;

        // Low stock count
        int lowStock = allInventoryItems.Count(i => i.IsLowStock);

        // Appointments by status
        var byStatus = new AppointmentsByStatus
        {
            Scheduled  = allAppointments.Count(a => a.Status == "Scheduled"),
            Confirmed  = allAppointments.Count(a => a.Status == "Confirmed"),
            Completed  = allAppointments.Count(a => a.Status == "Completed"),
            Cancelled  = allAppointments.Count(a => a.Status == "Cancelled"),
            NoShow     = allAppointments.Count(a => a.Status == "NoShow")
        };

        // Monthly appointments — last 6 months
        var monthly = new List<MonthlyAppointments>();
        for (int i = 5; i >= 0; i--)
        {
            var mStart = monthStart.AddMonths(-i);
            var mEnd   = mStart.AddMonths(1);
            int count  = allAppointments.Count(a => a.ScheduledAt >= mStart && a.ScheduledAt < mEnd);
            monthly.Add(new MonthlyAppointments
            {
                Month = mStart.ToString("MMM yyyy"),
                Count = count
            });
        }

        // Inventory by category
        var byCategory = allInventoryItems
            .GroupBy(i => i.Category)
            .Select(g => new InventoryByCategory
            {
                Category      = g.Key,
                TotalItems    = g.Count(),
                LowStockCount = g.Count(i => i.IsLowStock),
                TotalQuantity = g.Sum(i => i.Quantity)
            })
            .OrderBy(c => c.Category)
            .ToList();

        return new DashboardData
        {
            Overview = new OverviewStats
            {
                TotalPatients        = (int)totalPatientsTask.Result,
                ActivePatients       = (int)totalPatientsTask.Result,
                TotalStaff           = (int)staffTask.Result,
                AppointmentsToday    = (int)apptTodayTask.Result,
                AppointmentsThisMonth = (int)apptMonthTask.Result,
                AppointmentsPending  = (int)apptPendingTask.Result,
                LowStockItems        = lowStock,
                TotalInventoryItems  = (int)allInventoryTask.Result
            },
            AppointmentsByStatus = byStatus,
            MonthlyAppointments  = monthly,
            InventoryByCategory  = byCategory
        };
    }
}
