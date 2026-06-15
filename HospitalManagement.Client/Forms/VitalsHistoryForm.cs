using HospitalManagement.Client.Api;
using HospitalManagement.Shared.Models;
using System.Drawing;
using System.Windows.Forms;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Shows the vitals history for a single patient in a DataGridView.
/// Read-only, opens as a dialog from VitalsMonitorForm.
/// </summary>
public class VitalsHistoryForm : Form
{
    private readonly string _patientId;
    private DataGridView dgv = null!;

    public VitalsHistoryForm(string patientId)
    {
        _patientId = patientId;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.Text = "Vitals History";
        this.Size = new Size(820, 480);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.White;

        dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            Font = new Font("Segoe UI", 10),
            ColumnHeadersHeight = 38,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            }
        };
        dgv.EnableHeadersVisualStyles = false;

        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTime",   HeaderText = "Recorded At", FillWeight = 130 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colHR",     HeaderText = "HR (bpm)",    FillWeight = 80 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBP",     HeaderText = "BP",          FillWeight = 90 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTemp",   HeaderText = "Temp °C",     FillWeight = 80 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colO2",     HeaderText = "O₂ %",        FillWeight = 70 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Status",      FillWeight = 90 });

        this.Controls.Add(dgv);
        this.ResumeLayout(false);
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        try
        {
            var history = await ApiClient.GetAsync<List<PatientVitals>>($"/api/vitals/patient/{_patientId}?limit=20");
            if (history is null) return;

            this.Text = $"Vitals History — {history.FirstOrDefault()?.PatientName ?? _patientId}";

            foreach (var v in history)
            {
                int idx = dgv.Rows.Add(
                    v.RecordedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                    v.HeartRate,
                    $"{v.BloodPressureSystolic}/{v.BloodPressureDiastolic}",
                    v.Temperature.ToString("F1"),
                    v.OxygenSaturation.ToString("F1"),
                    v.IsCritical ? "🚨 CRITICAL" : "✓ Normal"
                );

                if (v.IsCritical)
                {
                    var row = dgv.Rows[idx];
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(183, 28, 28);
                }
            }
        }
        catch { /* graceful */ }
    }
}
