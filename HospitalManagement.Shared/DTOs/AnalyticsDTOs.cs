namespace HospitalManagement.Shared.DTOs;

/// <summary>Top-level summary cards for the dashboard.</summary>
public class OverviewStats
{
    public int TotalPatients { get; set; }
    public int ActivePatients { get; set; }
    public int TotalStaff { get; set; }
    public int AppointmentsToday { get; set; }
    public int AppointmentsThisMonth { get; set; }
    public int AppointmentsPending { get; set; }
    public int LowStockItems { get; set; }
    public int TotalInventoryItems { get; set; }
}

/// <summary>Count of appointments grouped by status.</summary>
public class AppointmentsByStatus
{
    public int Scheduled { get; set; }
    public int Confirmed { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
    public int NoShow { get; set; }
}

/// <summary>Appointments per month for the last N months.</summary>
public class MonthlyAppointments
{
    public string Month { get; set; } = string.Empty;  // e.g. "2026-05"
    public int Count { get; set; }
}

/// <summary>Inventory items grouped by category.</summary>
public class InventoryByCategory
{
    public string Category { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public int LowStockCount { get; set; }
    public int TotalQuantity { get; set; }
}

/// <summary>Full analytics payload returned by the dashboard endpoint.</summary>
public class DashboardData
{
    public OverviewStats Overview { get; set; } = new();
    public AppointmentsByStatus AppointmentsByStatus { get; set; } = new();
    public List<MonthlyAppointments> MonthlyAppointments { get; set; } = new();
    public List<InventoryByCategory> InventoryByCategory { get; set; } = new();
}
