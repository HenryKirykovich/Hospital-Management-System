using HospitalManagement.Client.Api;
using HospitalManagement.Client.Services;
using HospitalManagement.Client.Session;
using HospitalManagement.Shared.Models;
using System.Drawing;
using System.Windows.Forms;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Real-time patient vitals monitoring dashboard.
/// Shows latest vitals for all monitored patients.
/// Critical patients are highlighted red.
/// Staff (Admin/Doctor/Nurse) can record new vitals.
/// Updates in real-time via SignalR VitalsUpdate events.
/// </summary>
public class VitalsMonitorForm : Form
{
    private DataGridView dgvVitals = null!;
    private Button btnRefresh = null!;
    private Button btnRecord = null!;
    private Panel pnlNotification = null!;
    private Label lblNotification = null!;
    private System.Windows.Forms.Timer notificationTimer = null!;
    private Label lblLastUpdated = null!;

    private List<PatientVitals> _vitals = new();

    public VitalsMonitorForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.Text = "Vitals Monitor";
        this.BackColor = Color.White;

        // --- Notification bar ---
        pnlNotification = new Panel
        {
            Dock = DockStyle.Top,
            Height = 36,
            BackColor = Color.FromArgb(220, 53, 69),
            Visible = false
        };
        lblNotification = new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlNotification.Controls.Add(lblNotification);

        notificationTimer = new System.Windows.Forms.Timer { Interval = 6000 };
        notificationTimer.Tick += (s, e) => { pnlNotification.Visible = false; notificationTimer.Stop(); };

        // --- Toolbar ---
        var pnlToolbar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(248, 249, 250)
        };

        var lblTitle = new Label
        {
            Text = "Patient Vitals Monitor",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 37, 41),
            Location = new Point(12, 12),
            AutoSize = true
        };

        btnRefresh = new Button
        {
            Text = "↻ Refresh",
            Location = new Point(260, 10),
            Size = new Size(90, 30),
            BackColor = Color.FromArgb(0, 123, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9)
        };
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Click += async (s, e) => await LoadVitalsAsync();

        btnRecord = new Button
        {
            Text = "+ Record",
            Location = new Point(360, 10),
            Size = new Size(90, 30),
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9)
        };
        btnRecord.FlatAppearance.BorderSize = 0;
        btnRecord.Click += BtnRecord_Click;
        btnRecord.Visible = ClientSession.IsStaff;

        lblLastUpdated = new Label
        {
            Text = "",
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.Gray,
            Location = new Point(460, 16),
            AutoSize = true
        };

        pnlToolbar.Controls.AddRange(new Control[] { lblTitle, btnRefresh, btnRecord, lblLastUpdated });

        // --- DataGridView ---
        dgvVitals = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            ColumnHeadersHeight = 40,
            Font = new Font("Segoe UI", 10),
            GridColor = Color.FromArgb(222, 226, 230),
            DefaultCellStyle = new DataGridViewCellStyle
            {
                SelectionBackColor = Color.FromArgb(0, 123, 255),
                SelectionForeColor = Color.White,
                Padding = new Padding(4)
            },
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Padding = new Padding(4)
            }
        };
        dgvVitals.EnableHeadersVisualStyles = false;

        dgvVitals.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId",     HeaderText = "ID",      Visible = false });
        dgvVitals.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPatient", HeaderText = "Patient", FillWeight = 140 });
        dgvVitals.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRoom",    HeaderText = "Room",    FillWeight = 60 });
        dgvVitals.Columns.Add(new DataGridViewTextBoxColumn { Name = "colHR",      HeaderText = "HR (bpm)",FillWeight = 80 });
        dgvVitals.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBP",      HeaderText = "BP (mmHg)",FillWeight = 90 });
        dgvVitals.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTemp",    HeaderText = "Temp °C", FillWeight = 80 });
        dgvVitals.Columns.Add(new DataGridViewTextBoxColumn { Name = "colO2",      HeaderText = "O₂ Sat %",FillWeight = 80 });
        dgvVitals.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTime",    HeaderText = "Recorded",FillWeight = 110 });
        dgvVitals.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus",  HeaderText = "Status",  FillWeight = 80 });

        dgvVitals.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) ShowPatientHistory(); };

        this.Controls.Add(dgvVitals);
        this.Controls.Add(pnlToolbar);
        this.Controls.Add(pnlNotification);

        this.ResumeLayout(false);
    }

    // ====== Lifecycle ======

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (AppServices.SignalR is not null)
            AppServices.SignalR.OnVitalsUpdate += SignalR_VitalsUpdate;

        await LoadVitalsAsync();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
        notificationTimer.Stop();

        if (AppServices.SignalR is not null)
            AppServices.SignalR.OnVitalsUpdate -= SignalR_VitalsUpdate;
    }

    // ====== Data ======

    private async Task LoadVitalsAsync()
    {
        btnRefresh.Enabled = false;
        try
        {
            var result = await ApiClient.GetAsync<List<PatientVitals>>("/api/vitals");
            _vitals = result ?? new List<PatientVitals>();
            PopulateGrid(_vitals);
            lblLastUpdated.Text = $"Updated {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            ShowNotification($"Load error: {ex.Message}");
        }
        finally
        {
            btnRefresh.Enabled = true;
        }
    }

    private void PopulateGrid(List<PatientVitals> list)
    {
        dgvVitals.Rows.Clear();

        foreach (var v in list)
        {
            string bp = $"{v.BloodPressureSystolic}/{v.BloodPressureDiastolic}";
            string status = v.IsCritical ? "🚨 CRITICAL" : "✓ Normal";

            int rowIdx = dgvVitals.Rows.Add(
                v.Id,
                v.PatientName,
                v.RoomNumber,
                v.HeartRate,
                bp,
                v.Temperature.ToString("F1"),
                v.OxygenSaturation.ToString("F1"),
                v.RecordedAt.ToLocalTime().ToString("HH:mm:ss dd/MM"),
                status
            );

            if (v.IsCritical)
            {
                var row = dgvVitals.Rows[rowIdx];
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(183, 28, 28);
            }
        }

        int criticalCount = list.Count(v => v.IsCritical);
        this.Text = criticalCount > 0
            ? $"Vitals Monitor — {list.Count} patients  🚨 {criticalCount} CRITICAL"
            : $"Vitals Monitor — {list.Count} patients";
    }

    // ====== Record Vitals ======

    private void BtnRecord_Click(object? sender, EventArgs e)
    {
        using var form = new VitalsEntryForm();
        if (form.ShowDialog(this) == DialogResult.OK)
            _ = LoadVitalsAsync();
    }

    private void ShowPatientHistory()
    {
        if (dgvVitals.SelectedRows.Count == 0) return;
        string patientId = _vitals.ElementAtOrDefault(dgvVitals.SelectedRows[0].Index)?.PatientId ?? string.Empty;
        if (string.IsNullOrEmpty(patientId)) return;

        using var form = new VitalsHistoryForm(patientId);
        form.ShowDialog(this);
    }

    // ====== SignalR ======

    private void SignalR_VitalsUpdate(PatientVitals vitals)
    {
        InvokeOnUiThread(() =>
        {
            // Update existing row or insert at top if new patient
            bool found = false;
            foreach (DataGridViewRow row in dgvVitals.Rows)
            {
                if (row.Cells["colId"].Value?.ToString() == vitals.PatientId ||
                    row.Cells["colPatient"].Value?.ToString() == vitals.PatientName)
                {
                    string bp = $"{vitals.BloodPressureSystolic}/{vitals.BloodPressureDiastolic}";
                    row.Cells["colHR"].Value    = vitals.HeartRate;
                    row.Cells["colBP"].Value    = bp;
                    row.Cells["colTemp"].Value  = vitals.Temperature.ToString("F1");
                    row.Cells["colO2"].Value    = vitals.OxygenSaturation.ToString("F1");
                    row.Cells["colTime"].Value  = vitals.RecordedAt.ToLocalTime().ToString("HH:mm:ss dd/MM");
                    row.Cells["colStatus"].Value = vitals.IsCritical ? "🚨 CRITICAL" : "✓ Normal";
                    row.DefaultCellStyle.BackColor = vitals.IsCritical ? Color.FromArgb(255, 235, 238) : Color.White;
                    row.DefaultCellStyle.ForeColor = vitals.IsCritical ? Color.FromArgb(183, 28, 28) : Color.Black;
                    found = true;
                    break;
                }
            }

            if (!found)
                _ = LoadVitalsAsync();

            if (vitals.IsCritical)
                ShowNotification($"🚨 CRITICAL: {vitals.PatientName} (Room {vitals.RoomNumber}) — O₂:{vitals.OxygenSaturation}% HR:{vitals.HeartRate}");
        });
    }

    private void ShowNotification(string message)
    {
        lblNotification.Text = message;
        pnlNotification.Visible = true;
        notificationTimer.Stop();
        notificationTimer.Start();
    }

    private void InvokeOnUiThread(Action action)
    {
        if (this.IsDisposed || !this.IsHandleCreated) return;
        if (this.InvokeRequired) this.Invoke(action);
        else action();
    }
}
