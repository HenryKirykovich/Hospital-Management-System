using HospitalManagement.Client.Api;
using HospitalManagement.Shared.Models;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Add / Edit patient form.
/// Pass null to create a new patient, or an existing Patient to edit.
/// Returns DialogResult.OK when the patient was saved successfully.
/// </summary>
public partial class PatientEditForm : Form
{
    private readonly Patient? _existing;
    private readonly bool _isEdit;

    public PatientEditForm(Patient? patient)
    {
        InitializeComponent();
        _existing = patient;
        _isEdit = patient is not null;

        lblTitle.Text = _isEdit ? "Edit Patient" : "Add New Patient";
        if (_isEdit) PopulateFields(patient!);
    }

    // --- Populate for Edit ---

    private void PopulateFields(Patient p)
    {
        txtFirstName.Text = p.FirstName;
        txtLastName.Text = p.LastName;
        dtpDob.Value = p.DateOfBirth == DateTime.MinValue ? DateTime.Today.AddYears(-30) : p.DateOfBirth;
        cmbGender.SelectedItem = p.Gender;
        txtPhone.Text = p.Phone;
        txtEmail.Text = p.Email;
        txtAddress.Text = p.Address;
        cmbBloodType.SelectedItem = p.BloodType;
        txtAllergies.Text = string.Join(", ", p.Allergies);
    }

    // --- Save ---

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput()) return;

        SetBusy(true);
        try
        {
            var patient = BuildPatientFromForm();

            if (_isEdit)
            {
                await ApiClient.PutAsync($"/api/patient/{_existing!.Id}", patient);
            }
            else
            {
                await ApiClient.PostAsync<Patient>("/api/patient", patient);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving patient:\n{ex.Message}",
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

    // --- Helpers ---

    private Patient BuildPatientFromForm()
    {
        var allergiesRaw = txtAllergies.Text.Trim();
        var allergiesList = allergiesRaw.Length > 0
            ? allergiesRaw.Split(',').Select(a => a.Trim()).Where(a => a.Length > 0).ToList()
            : new List<string>();

        return new Patient
        {
            FirstName = txtFirstName.Text.Trim(),
            LastName = txtLastName.Text.Trim(),
            DateOfBirth = dtpDob.Value.Date,
            Gender = cmbGender.SelectedItem?.ToString() ?? string.Empty,
            Phone = txtPhone.Text.Trim(),
            Email = txtEmail.Text.Trim(),
            Address = txtAddress.Text.Trim(),
            BloodType = cmbBloodType.SelectedItem?.ToString() ?? string.Empty,
            Allergies = allergiesList,
            MedicalHistory = _existing?.MedicalHistory ?? new()
        };
    }

    private bool ValidateInput()
    {
        lblError.Visible = false;

        if (string.IsNullOrWhiteSpace(txtFirstName.Text))
        { ShowError("First name is required."); return false; }

        if (string.IsNullOrWhiteSpace(txtLastName.Text))
        { ShowError("Last name is required."); return false; }

        if (dtpDob.Value.Date >= DateTime.Today)
        { ShowError("Date of birth must be in the past."); return false; }

        return true;
    }

    private void SetBusy(bool busy)
    {
        btnSave.Enabled = !busy;
        btnCancel.Enabled = !busy;
        Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
    }

    private void ShowError(string message)
    {
        lblError.Text = message;
        lblError.Visible = true;
    }
}
