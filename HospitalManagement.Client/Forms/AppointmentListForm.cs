using System.Text.Json;
using HospitalManagement.Client.Api;
using HospitalManagement.Client.Session;
using HospitalManagement.Shared.Models;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Displays all appointments with real-time SignalR updates.
/// New/updated/cancelled appointments appear instantly without manual refresh.
/// </summary>
public partial class AppointmentListForm : Form
{
    private List<Appointment> _appointments = new();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AppointmentListForm()
    {
        InitializeComponent();
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Set initial button states
        btnAdd.Enabled = ClientSession.IsStaff;
        btnEdit.Enabled = false;
        btnDelete.Enabled = false;
        btnChangeStatus.Enabled = false;

        // Subscribe to real-time SignalR events
        AppServices.SignalR.OnAppointmentCreated += OnAppointmentCreatedRealTime;
        AppServices.SignalR.OnAppointmentUpdated += OnAppointmentUpdatedRealTime;
        AppServices.SignalR.OnAppointmentStatusChanged += OnStatusChangedRealTime;
        AppServices.SignalR.OnAppointmentDeleted += OnAppointmentDeletedRealTime;

        await LoadAppointmentsAsync();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        // Unsubscribe to prevent memory leaks when the form is replaced in the content panel
        AppServices.SignalR.OnAppointmentCreated -= OnAppointmentCreatedRealTime;
        AppServices.SignalR.OnAppointmentUpdated -= OnAppointmentUpdatedRealTime;
        AppServices.SignalR.OnAppointmentStatusChanged -= OnStatusChangedRealTime;
        AppServices.SignalR.OnAppointmentDeleted -= OnAppointmentDeletedRealTime;
        base.OnFormClosed(e);
    }

    // --- Data Loading ---

    private async Task LoadAppointmentsAsync()
    {
        SetBusy(true);
        try
        {
            _appointments = await ApiClient.GetAsync<List<Appointment>>("/api/appointment") ?? new();
            BindGrid();
            lblCount.Text = $"{_appointments.Count} appointment(s)";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading appointments:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void BindGrid()
    {
        dgvAppointments.DataSource = _appointments.Select(a => new
        {
            a.Id,
            Patient = a.PatientName,
            Doctor = a.DoctorName,
            Date = a.ScheduledAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
            Duration = $"{a.DurationMinutes} min",
            a.Reason,
            Status = a.Status,
            a.Notes
        }).ToList();

        if (dgvAppointments.Columns["Id"] is DataGridViewColumn col)
            col.Visible = false;

        ColorStatusRows();
    }

    /// <summary>Color-codes rows based on appointment status</summary>
    private void ColorStatusRows()
    {
        foreach (DataGridViewRow row in dgvAppointments.Rows)
        {
            var status = row.Cells["Status"].Value?.ToString();
            row.DefaultCellStyle.BackColor = status switch
            {
                "Confirmed" => Color.FromArgb(220, 255, 220),
                "Cancelled" => Color.FromArgb(255, 220, 220),
                "Completed" => Color.FromArgb(220, 220, 255),
                "NoShow" => Color.FromArgb(255, 240, 200),
                _ => Color.White
            };
        }
    }

    // --- Real-time SignalR Handlers ---

    private void OnAppointmentCreatedRealTime(object data)
    {
        InvokeOnUiThread(() =>
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var appt = JsonSerializer.Deserialize<Appointment>(json, _jsonOptions);
                if (appt is not null)
                {
                    _appointments.Add(appt);
                    BindGrid();
                    lblCount.Text = $"{_appointments.Count} appointment(s)";
                    ShowNotification($"New appointment: {appt.PatientName} → Dr. {appt.DoctorName}");
                }
            }
            catch { /* Ignore deserialization errors */ }
        });
    }

    private void OnAppointmentUpdatedRealTime(object data)
    {
        InvokeOnUiThread(() =>
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var appt = JsonSerializer.Deserialize<Appointment>(json, _jsonOptions);
                if (appt is not null)
                {
                    var idx = _appointments.FindIndex(a => a.Id == appt.Id);
                    if (idx >= 0) _appointments[idx] = appt;
                    BindGrid();
                }
            }
            catch { /* Ignore deserialization errors */ }
        });
    }

    private void OnStatusChangedRealTime(object data)
    {
        InvokeOnUiThread(() =>
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                using var doc = JsonDocument.Parse(json);
                var id = doc.RootElement.GetProperty("appointmentId").GetString();
                var status = doc.RootElement.GetProperty("status").GetString();

                var appt = _appointments.FirstOrDefault(a => a.Id == id);
                if (appt is not null)
                {
                    appt.Status = status ?? appt.Status;
                    BindGrid();
                    ShowNotification($"Appointment status changed to: {status}");
                }
            }
            catch { /* Ignore deserialization errors */ }
        });
    }

    private void OnAppointmentDeletedRealTime(string id)
    {
        InvokeOnUiThread(() =>
        {
            _appointments.RemoveAll(a => a.Id == id);
            BindGrid();
            lblCount.Text = $"{_appointments.Count} appointment(s)";
        });
    }

    // --- Button Handlers ---

    private void btnAdd_Click(object sender, EventArgs e)
    {
        var form = new AppointmentEditForm(null);
        if (form.ShowDialog(this) == DialogResult.OK)
            _ = LoadAppointmentsAsync();
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var appt = GetSelected();
        if (appt is null) return;
        var form = new AppointmentEditForm(appt);
        if (form.ShowDialog(this) == DialogResult.OK)
            _ = LoadAppointmentsAsync();
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var appt = GetSelected();
        if (appt is null) return;

        var confirm = MessageBox.Show(
            $"Delete appointment for {appt.PatientName} on {appt.ScheduledAt.ToLocalTime():yyyy-MM-dd HH:mm}?",
            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        try
        {
            await ApiClient.DeleteAsync($"/api/appointment/{appt.Id}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting appointment:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnChangeStatus_Click(object sender, EventArgs e)
    {
        var appt = GetSelected();
        if (appt is null) return;

        var statuses = new[] { "Scheduled", "Confirmed", "Cancelled", "Completed", "NoShow" };
        using var dlg = new StatusPickerDialog(appt.Status, statuses);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            await ApiClient.PostAsync<object>($"/api/appointment/{appt.Id}/status",
                new { status = dlg.SelectedStatus });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating status:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnRefresh_Click(object sender, EventArgs e) =>
        await LoadAppointmentsAsync();

    private void dgvAppointments_SelectionChanged(object sender, EventArgs e)
    {
        var hasSelection = dgvAppointments.SelectedRows.Count > 0;
        btnEdit.Enabled = hasSelection && ClientSession.IsStaff;
        btnDelete.Enabled = hasSelection && (ClientSession.IsAdmin || ClientSession.IsDoctor);
        btnChangeStatus.Enabled = hasSelection;
    }

    private void dgvAppointments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) btnEdit_Click(sender, e);
    }

    // --- Helpers ---

    private Appointment? GetSelected()
    {
        if (dgvAppointments.SelectedRows.Count == 0) return null;
        var id = dgvAppointments.SelectedRows[0].Cells["Id"].Value?.ToString();
        return _appointments.FirstOrDefault(a => a.Id == id);
    }

    private void SetBusy(bool busy)
    {
        btnAdd.Enabled = !busy && ClientSession.IsStaff;
        btnRefresh.Enabled = !busy;
        Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        lblStatus.Visible = busy;
    }

    /// <summary>Shows a brief notification bar at the top of the grid</summary>
    private void ShowNotification(string message)
    {
        lblNotification.Text = $"🔔 {message}";
        lblNotification.Visible = true;
        notificationTimer.Stop();
        notificationTimer.Start();
    }

    private void notificationTimer_Tick(object sender, EventArgs e)
    {
        notificationTimer.Stop();
        lblNotification.Visible = false;
    }

    /// <summary>Marshals the action back to the UI thread from a SignalR background thread</summary>
    private void InvokeOnUiThread(Action action)
    {
        if (IsDisposed || !IsHandleCreated) return;
        if (InvokeRequired) Invoke(action);
        else action();
    }
}
