using HospitalManagement.Client.Session;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Main application window — shown after a successful login.
/// Contains a sidebar for navigation and a content panel that hosts feature forms.
/// </summary>
public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        LoadUserInfo();
        BuildSidebarMenu();
    }

    /// <summary>Displays the logged-in user's name and role in the header</summary>
    private void LoadUserInfo()
    {
        lblWelcome.Text = $"Welcome, {ClientSession.FullName}  |  Role: {ClientSession.Role}";
    }

    /// <summary>
    /// Dynamically builds sidebar navigation buttons based on available features.
    /// New features (Appointments, Inventory, etc.) are added here as stages complete.
    /// </summary>
    private void BuildSidebarMenu()
    {
        var menuItems = new List<(string Text, Action OnClick)>
        {
            ("🏠  Dashboard",    () => OpenForm(new DashboardForm())),
            ("👥  Patients",     () => OpenForm(new PatientListForm())),
            ("📅  Appointments", () => OpenForm(new AppointmentListForm())),
            ("💊  Inventory",    () => OpenForm(new InventoryListForm())),
            ("📊  Analytics",    () => OpenForm(new DashboardForm())),
            ("💬  Chat",         () => ShowPlaceholder("Chat — coming in Stage 7")),
        };

        int y = 60;
        foreach (var (text, onClick) in menuItems)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(0, y),
                Size = new Size(220, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(22, 55, 120),
                ForeColor = Color.FromArgb(200, 220, 255),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 80, 160);
            var action = onClick;
            btn.Click += (_, _) => action();
            pnlSidebar.Controls.Add(btn);
            y += 44;
        }
    }

    /// <summary>Loads a form into the content panel (replaces previous content)</summary>
    private void OpenForm(Form form)
    {
        pnlContent.Controls.Clear();
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(form);
        form.Show();
    }

    private void ShowPlaceholder(string message)
    {
        pnlContent.Controls.Clear();
        var lbl = new Label
        {
            Text = message,
            Font = new Font("Segoe UI", 12F),
            ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill
        };
        pnlContent.Controls.Add(lbl);
    }

    private void btnLogout_Click(object sender, EventArgs e)
    {
        var confirm = MessageBox.Show("Are you sure you want to log out?",
            "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            _ = AppServices.DisposeAsync();
            ClientSession.Clear();
            var loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
    }
}
