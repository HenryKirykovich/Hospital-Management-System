namespace HospitalManagement.Client.Forms;

partial class LoginForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.pnlMain = new Panel();
        this.lblTitle = new Label();
        this.lblSubtitle = new Label();
        this.lblUsernameLabel = new Label();
        this.txtUsername = new TextBox();
        this.lblPasswordLabel = new Label();
        this.txtPassword = new TextBox();
        this.btnLogin = new Button();
        this.btnRegister = new Button();
        this.lblError = new Label();
        this.lblStatus = new Label();
        this.pnlMain.SuspendLayout();
        this.SuspendLayout();

        // --- Form ---
        this.Text = "Hospital Management System — Login";
        this.Size = new Size(440, 520);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(240, 244, 248);
        this.Font = new Font("Segoe UI", 10F);

        // --- pnlMain (white card) ---
        this.pnlMain.BackColor = Color.White;
        this.pnlMain.BorderStyle = BorderStyle.None;
        this.pnlMain.Location = new Point(40, 40);
        this.pnlMain.Size = new Size(360, 420);
        this.pnlMain.Padding = new Padding(30);

        // --- lblTitle ---
        this.lblTitle.Text = "Hospital Management";
        this.lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        this.lblTitle.ForeColor = Color.FromArgb(30, 80, 160);
        this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        this.lblTitle.Location = new Point(20, 30);
        this.lblTitle.Size = new Size(320, 36);

        // --- lblSubtitle ---
        this.lblSubtitle.Text = "Sign in to your account";
        this.lblSubtitle.Font = new Font("Segoe UI", 10F);
        this.lblSubtitle.ForeColor = Color.Gray;
        this.lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
        this.lblSubtitle.Location = new Point(20, 70);
        this.lblSubtitle.Size = new Size(320, 24);

        // --- lblUsernameLabel ---
        this.lblUsernameLabel.Text = "Username";
        this.lblUsernameLabel.ForeColor = Color.FromArgb(60, 60, 60);
        this.lblUsernameLabel.Location = new Point(20, 115);
        this.lblUsernameLabel.Size = new Size(320, 20);

        // --- txtUsername ---
        this.txtUsername.Location = new Point(20, 138);
        this.txtUsername.Size = new Size(320, 32);
        this.txtUsername.BorderStyle = BorderStyle.FixedSingle;
        this.txtUsername.Font = new Font("Segoe UI", 11F);

        // --- lblPasswordLabel ---
        this.lblPasswordLabel.Text = "Password";
        this.lblPasswordLabel.ForeColor = Color.FromArgb(60, 60, 60);
        this.lblPasswordLabel.Location = new Point(20, 185);
        this.lblPasswordLabel.Size = new Size(320, 20);

        // --- txtPassword ---
        this.txtPassword.Location = new Point(20, 208);
        this.txtPassword.Size = new Size(320, 32);
        this.txtPassword.BorderStyle = BorderStyle.FixedSingle;
        this.txtPassword.Font = new Font("Segoe UI", 11F);
        this.txtPassword.UseSystemPasswordChar = true;
        this.txtPassword.KeyDown += new KeyEventHandler(this.txtPassword_KeyDown);

        // --- lblError ---
        this.lblError.ForeColor = Color.FromArgb(200, 50, 50);
        this.lblError.Font = new Font("Segoe UI", 9F);
        this.lblError.Location = new Point(20, 250);
        this.lblError.Size = new Size(320, 36);
        this.lblError.Visible = false;
        this.lblError.TextAlign = ContentAlignment.MiddleLeft;

        // --- btnLogin ---
        this.btnLogin.Text = "Sign In";
        this.btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        this.btnLogin.BackColor = Color.FromArgb(30, 80, 160);
        this.btnLogin.ForeColor = Color.White;
        this.btnLogin.FlatStyle = FlatStyle.Flat;
        this.btnLogin.FlatAppearance.BorderSize = 0;
        this.btnLogin.Location = new Point(20, 295);
        this.btnLogin.Size = new Size(320, 42);
        this.btnLogin.Cursor = Cursors.Hand;
        this.btnLogin.Click += new EventHandler(this.btnLogin_Click);

        // --- btnRegister ---
        this.btnRegister.Text = "Create new account";
        this.btnRegister.Font = new Font("Segoe UI", 9F);
        this.btnRegister.BackColor = Color.White;
        this.btnRegister.ForeColor = Color.FromArgb(30, 80, 160);
        this.btnRegister.FlatStyle = FlatStyle.Flat;
        this.btnRegister.FlatAppearance.BorderColor = Color.FromArgb(30, 80, 160);
        this.btnRegister.Location = new Point(20, 348);
        this.btnRegister.Size = new Size(320, 36);
        this.btnRegister.Cursor = Cursors.Hand;
        this.btnRegister.Click += new EventHandler(this.btnRegister_Click);

        // --- lblStatus ---
        this.lblStatus.Text = "Connecting...";
        this.lblStatus.ForeColor = Color.Gray;
        this.lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        this.lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        this.lblStatus.Location = new Point(20, 390);
        this.lblStatus.Size = new Size(320, 20);
        this.lblStatus.Visible = false;

        // --- Assemble ---
        this.pnlMain.Controls.AddRange(new Control[]
        {
            lblTitle, lblSubtitle,
            lblUsernameLabel, txtUsername,
            lblPasswordLabel, txtPassword,
            lblError, btnLogin, btnRegister, lblStatus
        });
        this.Controls.Add(pnlMain);

        this.pnlMain.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    // --- Controls ---
    private Panel pnlMain;
    private Label lblTitle;
    private Label lblSubtitle;
    private Label lblUsernameLabel;
    private TextBox txtUsername;
    private Label lblPasswordLabel;
    private TextBox txtPassword;
    private Button btnLogin;
    private Button btnRegister;
    private Label lblError;
    private Label lblStatus;
}
