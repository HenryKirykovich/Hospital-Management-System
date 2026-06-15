using HospitalManagement.Client.Api;
using HospitalManagement.Client.Session;
using HospitalManagement.Shared.Models;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Displays all patients in a DataGridView with search, add, edit, and delete actions.
/// Loaded inside the MainForm content panel.
/// </summary>
public partial class PatientListForm : Form
{
    private List<Patient> _patients = new();

    public PatientListForm()
    {
        InitializeComponent();
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await LoadPatientsAsync();
    }

    // --- Data Loading ---

    private async Task LoadPatientsAsync(string searchQuery = "")
    {
        SetBusy(true);
        try
        {
            var path = string.IsNullOrWhiteSpace(searchQuery)
                ? "/api/patient"
                : $"/api/patient/search?q={Uri.EscapeDataString(searchQuery)}";

            _patients = await ApiClient.GetAsync<List<Patient>>(path) ?? new();
            BindGrid();
            lblCount.Text = $"{_patients.Count} patient(s) found";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading patients:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void BindGrid()
    {
        var display = _patients.Select(p => new
        {
            p.Id,
            FullName = $"{p.FirstName} {p.LastName}",
            p.DateOfBirth,
            Age = CalculateAge(p.DateOfBirth),
            p.Gender,
            p.Phone,
            p.Email,
            p.BloodType,
            Records = p.MedicalHistory.Count
        }).ToList();

        dgvPatients.DataSource = display;

        // Hide the raw Id column from the user
        if (dgvPatients.Columns["Id"] is DataGridViewColumn col)
            col.Visible = false;
    }

    // --- Event Handlers ---

    private async void txtSearch_TextChanged(object sender, EventArgs e)
    {
        // Debounce: only search when user stops typing for 400ms
        searchTimer.Stop();
        searchTimer.Start();
    }

    private async void searchTimer_Tick(object sender, EventArgs e)
    {
        searchTimer.Stop();
        await LoadPatientsAsync(txtSearch.Text.Trim());
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        var editForm = new PatientEditForm(null);
        if (editForm.ShowDialog(this) == DialogResult.OK)
            _ = LoadPatientsAsync();
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var patient = GetSelectedPatient();
        if (patient is null) return;

        var editForm = new PatientEditForm(patient);
        if (editForm.ShowDialog(this) == DialogResult.OK)
            _ = LoadPatientsAsync();
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var patient = GetSelectedPatient();
        if (patient is null) return;

        var confirm = MessageBox.Show(
            $"Delete patient '{patient.FirstName} {patient.LastName}'?\nThis action cannot be undone.",
            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes) return;

        try
        {
            await ApiClient.DeleteAsync($"/api/patient/{patient.Id}");
            await LoadPatientsAsync(txtSearch.Text.Trim());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting patient:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnHistory_Click(object sender, EventArgs e)
    {
        var patient = GetSelectedPatient();
        if (patient is null) return;

        var histForm = new MedicalHistoryForm(patient);
        histForm.ShowDialog(this);
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        txtSearch.Clear();
        await LoadPatientsAsync();
    }

    private void dgvPatients_SelectionChanged(object sender, EventArgs e)
    {
        var hasSelection = dgvPatients.SelectedRows.Count > 0;
        // Admin and Doctor roles can delete; all staff can edit
        btnDelete.Enabled = hasSelection && (ClientSession.IsAdmin || ClientSession.IsDoctor);
        btnEdit.Enabled = hasSelection && ClientSession.IsStaff;
        btnHistory.Enabled = hasSelection;
    }

    private void dgvPatients_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) btnEdit_Click(sender, e);
    }

    // --- Helpers ---

    private Patient? GetSelectedPatient()
    {
        if (dgvPatients.SelectedRows.Count == 0) return null;
        var id = dgvPatients.SelectedRows[0].Cells["Id"].Value?.ToString();
        return _patients.FirstOrDefault(p => p.Id == id);
    }

    private static int CalculateAge(DateTime dob)
    {
        var today = DateTime.Today;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age;
    }

    private void SetBusy(bool busy)
    {
        btnAdd.Enabled = !busy && ClientSession.IsStaff;
        btnRefresh.Enabled = !busy;
        Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        lblStatus.Visible = busy;
    }
}
