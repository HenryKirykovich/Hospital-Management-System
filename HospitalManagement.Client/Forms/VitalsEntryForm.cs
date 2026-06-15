using HospitalManagement.Client.Api;
using HospitalManagement.Shared.Models;
using System.Drawing;
using System.Windows.Forms;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Form for recording new patient vitals.
/// Normal ranges shown as hints.
/// Server auto-detects critical status.
/// </summary>
public class VitalsEntryForm : Form
{
    private TextBox txtPatientId = null!;
    private TextBox txtPatientName = null!;
    private TextBox txtRoom = null!;
    private NumericUpDown nudHR = null!;
    private NumericUpDown nudSystolic = null!;
    private NumericUpDown nudDiastolic = null!;
    private NumericUpDown nudTemp = null!;
    private NumericUpDown nudO2 = null!;
    private Button btnSave = null!;
    private Button btnCancel = null!;
    private Label lblError = null!;

    public VitalsEntryForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.Text = "Record Patient Vitals";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.White;
        this.Size = new Size(420, 420);

        int lx = 20, fx = 200, fw = 180, rh = 36, y = 20;

        Label L(string t) => new Label { Text = t, Location = new Point(lx, y + 3), Size = new Size(170, 24), Font = new Font("Segoe UI", 10) };
        Control Row(string label, Control ctrl) { this.Controls.Add(L(label)); ctrl.Location = new Point(fx, y); this.Controls.Add(ctrl); y += rh; return ctrl; }

        txtPatientId = new TextBox { Size = new Size(fw, 28), Font = new Font("Segoe UI", 10), PlaceholderText = "MongoDB ObjectId" };
        Row("Patient ID *", txtPatientId);

        txtPatientName = new TextBox { Size = new Size(fw, 28), Font = new Font("Segoe UI", 10) };
        Row("Patient Name *", txtPatientName);

        txtRoom = new TextBox { Size = new Size(fw, 28), Font = new Font("Segoe UI", 10), Text = "101" };
        Row("Room Number", txtRoom);

        nudHR = new NumericUpDown { Size = new Size(fw, 28), Font = new Font("Segoe UI", 10), Minimum = 20, Maximum = 250, Value = 75 };
        Row("Heart Rate (bpm)", nudHR);

        nudSystolic = new NumericUpDown { Size = new Size(fw, 28), Font = new Font("Segoe UI", 10), Minimum = 50, Maximum = 250, Value = 120 };
        Row("BP Systolic", nudSystolic);

        nudDiastolic = new NumericUpDown { Size = new Size(fw, 28), Font = new Font("Segoe UI", 10), Minimum = 30, Maximum = 150, Value = 80 };
        Row("BP Diastolic", nudDiastolic);

        nudTemp = new NumericUpDown { Size = new Size(fw, 28), Font = new Font("Segoe UI", 10), Minimum = 30, Maximum = 45, Value = 37, DecimalPlaces = 1 };
        Row("Temperature (°C)", nudTemp);

        nudO2 = new NumericUpDown { Size = new Size(fw, 28), Font = new Font("Segoe UI", 10), Minimum = 50, Maximum = 100, Value = 98, DecimalPlaces = 1 };
        Row("O₂ Saturation (%)", nudO2);

        lblError = new Label { Location = new Point(lx, y), Size = new Size(360, 24), ForeColor = Color.Red, Font = new Font("Segoe UI", 9) };
        this.Controls.Add(lblError);
        y += 28;

        btnSave = new Button { Text = "Record", Location = new Point(fx, y), Size = new Size(90, 32), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Click += BtnSave_Click;

        btnCancel = new Button { Text = "Cancel", Location = new Point(fx + 100, y), Size = new Size(80, 32), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9) };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        this.Controls.AddRange(new Control[] { btnSave, btnCancel });
        this.ClientSize = new Size(400, y + 60);
        this.ResumeLayout(false);
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        lblError.Text = string.Empty;

        if (string.IsNullOrWhiteSpace(txtPatientId.Text))
        { lblError.Text = "Patient ID is required."; return; }
        if (string.IsNullOrWhiteSpace(txtPatientName.Text))
        { lblError.Text = "Patient Name is required."; return; }

        var vitals = new PatientVitals
        {
            PatientId              = txtPatientId.Text.Trim(),
            PatientName            = txtPatientName.Text.Trim(),
            RoomNumber             = txtRoom.Text.Trim(),
            HeartRate              = (double)nudHR.Value,
            BloodPressureSystolic  = (double)nudSystolic.Value,
            BloodPressureDiastolic = (double)nudDiastolic.Value,
            Temperature            = (double)nudTemp.Value,
            OxygenSaturation       = (double)nudO2.Value
        };

        btnSave.Enabled = false;
        try
        {
            await ApiClient.PostAsync<PatientVitals>("/api/vitals", vitals);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            btnSave.Enabled = true;
        }
    }
}
