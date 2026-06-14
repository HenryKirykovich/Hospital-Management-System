using HospitalManagement.Client.Session;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Main application window — shown after a successful login.
/// Acts as the navigation shell; individual feature panels will be added in later stages.
/// </summary>
public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        LoadUserInfo();
    }

    /// <summary>Displays the logged-in user's name and role in the status bar</summary>
    private void LoadUserInfo()
    {
        lblWelcome.Text = $"Welcome, {ClientSession.FullName}  |  Role: {ClientSession.Role}";
    }

    private void btnLogout_Click(object sender, EventArgs e)
    {
        var confirm = MessageBox.Show("Are you sure you want to log out?",
            "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            ClientSession.Clear();
            var loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
    }
}
