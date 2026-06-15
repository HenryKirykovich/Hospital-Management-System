using HospitalManagement.Client.Api;
using HospitalManagement.Client.Session;
using HospitalManagement.Shared.Models;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Displays the medical history for a specific patient
/// and allows Doctors/Admins to add new entries.
/// </summary>
public partial class MedicalHistoryForm : Form
{
    private readonly Patient _patient;

    public MedicalHistoryForm(Patient patient)
    {
        InitializeComponent();
        _patient = patient;
        lblPatientName.Text = $"Medical History — {patient.FirstName} {patient.LastName}";
        LoadHistory();
    }

    private void LoadHistory()
    {
        dgvHistory.DataSource = _patient.MedicalHistory
            .OrderByDescending(r => r.Date)
            .Select(r => new
            {
                Date = r.Date.ToLocalTime().ToString("yyyy-MM-dd"),
                r.Diagnosis,
                r.Treatment,
                r.Notes
            }).ToList();
    }

    private async void btnAddRecord_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtDiagnosis.Text))
        {
            MessageBox.Show("Diagnosis is required.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var record = new MedicalRecord
        {
            Diagnosis = txtDiagnosis.Text.Trim(),
            Treatment = txtTreatment.Text.Trim(),
            Notes = txtNotes.Text.Trim(),
            DoctorId = ClientSession.UserId
        };

        try
        {
            await ApiClient.PostAsync<object>($"/api/patient/{_patient.Id}/medical-record", record);

            // Add to local list and refresh grid without reloading from server
            _patient.MedicalHistory.Add(record);
            LoadHistory();

            txtDiagnosis.Clear();
            txtTreatment.Clear();
            txtNotes.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving record:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnClose_Click(object sender, EventArgs e) => Close();
}
