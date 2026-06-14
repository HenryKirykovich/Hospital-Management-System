using HospitalManagement.Client.Api;
using HospitalManagement.Client.Session;
using HospitalManagement.Shared.DTOs;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Login form — the entry point of the application.
/// Authenticates the user against the server and stores the JWT in ClientSession.
/// </summary>
public partial class LoginForm : Form
{
    public LoginForm()
    {
        InitializeComponent();
    }

    // --- Event Handlers ---

    private async void btnLogin_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
            return;

        SetBusy(true);

        try
        {
            var request = new LoginRequest
            {
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text
            };

            var response = await ApiClient.PostAsync<AuthResponse>("/api/auth/login", request);

            if (response is null)
            {
                ShowError("Login failed. Please try again.");
                return;
            }

            // Store session data globally
            ClientSession.SetSession(response);

            // Open the main form and close the login window
            var mainForm = new MainForm();
            mainForm.Show();
            this.Hide();
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("401"))
        {
            ShowError("Invalid username or password.");
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

    private void btnRegister_Click(object sender, EventArgs e)
    {
        // Open register form
        var registerForm = new RegisterForm();
        registerForm.ShowDialog(this);
    }

    private void txtPassword_KeyDown(object sender, KeyEventArgs e)
    {
        // Allow pressing Enter in the password field to trigger login
        if (e.KeyCode == Keys.Enter)
            btnLogin_Click(sender, e);
    }

    // --- Helpers ---

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            ShowError("Please enter your username.");
            txtUsername.Focus();
            return false;
        }
        if (string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            ShowError("Please enter your password.");
            txtPassword.Focus();
            return false;
        }
        lblError.Visible = false;
        return true;
    }

    private void SetBusy(bool busy)
    {
        btnLogin.Enabled = !busy;
        btnRegister.Enabled = !busy;
        txtUsername.Enabled = !busy;
        txtPassword.Enabled = !busy;
        lblStatus.Text = busy ? "Connecting..." : string.Empty;
        lblStatus.Visible = busy;
        Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
    }

    private void ShowError(string message)
    {
        lblError.Text = message;
        lblError.Visible = true;
    }
}
