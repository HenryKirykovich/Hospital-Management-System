using HospitalManagement.Client.Api;
using HospitalManagement.Client.Session;
using HospitalManagement.Shared.DTOs;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Registration form — allows creating a new user account.
/// On success, automatically logs in and opens the main form.
/// </summary>
public partial class RegisterForm : Form
{
    public RegisterForm()
    {
        InitializeComponent();
    }

    private async void btnRegister_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
            return;

        SetBusy(true);

        try
        {
            var request = new RegisterRequest
            {
                Username = txtUsername.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Password = txtPassword.Text,
                FullName = txtFullName.Text.Trim(),
                Role = cmbRole.SelectedItem?.ToString() ?? "Patient"
            };

            var response = await ApiClient.PostAsync<AuthResponse>("/api/auth/register", request);

            if (response is null)
            {
                ShowError("Registration failed. Please try again.");
                return;
            }

            // Auto-login after successful registration
            ClientSession.SetSession(response);

            MessageBox.Show($"Welcome, {response.FullName}!\nYour account has been created.",
                "Registration Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var mainForm = new MainForm();
            mainForm.Show();

            // Close both register and login forms
            this.Owner?.Hide();
            this.Close();
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("409"))
        {
            ShowError("Username or email is already taken.");
        }
        catch (Exception ex)
        {
            ShowError($"Cannot connect to server.\n{ex.Message}");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void btnCancel_Click(object sender, EventArgs e) => this.Close();

    private bool ValidateInput()
    {
        lblError.Visible = false;

        if (string.IsNullOrWhiteSpace(txtFullName.Text))
        { ShowError("Full name is required."); return false; }

        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        { ShowError("Username is required."); return false; }

        if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains('@'))
        { ShowError("A valid email address is required."); return false; }

        if (txtPassword.Text.Length < 6)
        { ShowError("Password must be at least 6 characters."); return false; }

        if (txtPassword.Text != txtConfirmPassword.Text)
        { ShowError("Passwords do not match."); return false; }

        return true;
    }

    private void SetBusy(bool busy)
    {
        btnRegister.Enabled = !busy;
        btnCancel.Enabled = !busy;
        Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
    }

    private void ShowError(string message)
    {
        lblError.Text = message;
        lblError.Visible = true;
    }
}
