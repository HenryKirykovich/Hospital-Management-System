using HospitalManagement.Client.Api;
using HospitalManagement.Shared.Models;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Add / Edit appointment form.
/// Loads the patient and doctor lists from the server for dropdown selection.
/// </summary>
public partial class AppointmentEditForm : Form
{
    private readonly Appointment? _existing;
    private readonly bool _isEdit;
    private List<Patient> _patients = new();
    private List<User> _doctors = new();

    public AppointmentEditForm(Appointment? appointment)
    {
        InitializeComponent();
        _existing = appointment;
        _isEdit = appointment is not null;
        lblTitle.Text = _isEdit ? "Edit Appointment" : "New Appointment";
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await LoadDropdownsAsync();
        if (_isEdit) PopulateFields(_existing!);
    }

    private async Task LoadDropdownsAsync()
    {
        try
        {
            _patients = await ApiClient.GetAsync<List<Patient>>("/api/patient") ?? new();
            cmbPatient.DataSource = _patients;
            cmbPatient.DisplayMember = "FullNameDisplay";
            cmbPatient.ValueMember = "Id";

            _doctors = await ApiClient.GetAsync<List<User>>("/api/user/doctors") ?? new();
            cmbDoctor.DataSource = _doctors;
            cmbDoctor.DisplayMember = "FullName";
            cmbDoctor.ValueMember = "Id";
        }
        catch
        {
            // If endpoint not yet available, dropdowns stay empty — user can type manually
        }
    }

    private void PopulateFields(Appointment a)
    {
        dtpDate.Value = a.ScheduledAt.ToLocalTime();
        nudDuration.Value = a.DurationMinutes;
        txtReason.Text = a.Reason;
        txtNotes.Text = a.Notes;
        cmbStatus.SelectedItem = a.Status;

        // Try to pre-select patient and doctor in dropdowns
        var patient = _patients.FirstOrDefault(p => p.Id == a.PatientId);
        if (patient is not null) cmbPatient.SelectedItem = patient;
        else txtPatientName.Text = a.PatientName;

        var doctor = _doctors.FirstOrDefault(d => d.Id == a.DoctorId);
        if (doctor is not null) cmbDoctor.SelectedItem = doctor;
        else txtDoctorName.Text = a.DoctorName;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput()) return;
        SetBusy(true);
        try
        {
            var appointment = BuildAppointment();
            if (_isEdit)
                await ApiClient.PutAsync($"/api/appointment/{_existing!.Id}", appointment);
            else
                await ApiClient.PostAsync<Appointment>("/api/appointment", appointment);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving appointment:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private Appointment BuildAppointment()
    {
        // Use dropdown selection if available, fall back to manual text input
        var selectedPatient = cmbPatient.SelectedItem as Patient;
        var selectedDoctor = cmbDoctor.SelectedItem as User;

        return new Appointment
        {
            PatientId = selectedPatient?.Id ?? string.Empty,
            PatientName = selectedPatient is not null
                ? $"{selectedPatient.FirstName} {selectedPatient.LastName}"
                : txtPatientName.Text.Trim(),
            DoctorId = selectedDoctor?.Id ?? string.Empty,
            DoctorName = selectedDoctor?.FullName ?? txtDoctorName.Text.Trim(),
            ScheduledAt = dtpDate.Value.ToUniversalTime(),
            DurationMinutes = (int)nudDuration.Value,
            Reason = txtReason.Text.Trim(),
            Notes = txtNotes.Text.Trim(),
            Status = cmbStatus.SelectedItem?.ToString() ?? "Scheduled"
        };
    }

    private bool ValidateInput()
    {
        lblError.Visible = false;

        var hasPatient = cmbPatient.SelectedItem is not null || !string.IsNullOrWhiteSpace(txtPatientName.Text);
        if (!hasPatient) { ShowError("Please select or enter a patient."); return false; }

        var hasDoctor = cmbDoctor.SelectedItem is not null || !string.IsNullOrWhiteSpace(txtDoctorName.Text);
        if (!hasDoctor) { ShowError("Please select or enter a doctor."); return false; }

        if (dtpDate.Value < DateTime.Now.AddMinutes(-5))
        { ShowError("Appointment date must be in the future."); return false; }

        return true;
    }

    private void SetBusy(bool busy)
    {
        btnSave.Enabled = !busy;
        btnCancel.Enabled = !busy;
        Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
    }

    private void ShowError(string msg) { lblError.Text = msg; lblError.Visible = true; }
}
