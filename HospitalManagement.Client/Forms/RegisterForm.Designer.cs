namespace HospitalManagement.Client.Forms;

partial class RegisterForm
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
        this.lblTitle = new Label();
        this.lblFullNameLabel = new Label();
        this.txtFullName = new TextBox();
        this.lblUsernameLabel = new Label();
        this.txtUsername = new TextBox();
        this.lblEmailLabel = new Label();
        this.txtEmail = new TextBox();
        this.lblPasswordLabel = new Label();
        this.txtPassword = new TextBox();
        this.lblConfirmPasswordLabel = new Label();
        this.txtConfirmPassword = new TextBox();
        this.lblRoleLabel = new Label();
        this.cmbRole = new ComboBox();
        this.btnRegister = new Button();
        this.btnCancel = new Button();
        this.lblError = new Label();
        this.SuspendLayout();

        // --- Form ---
        this.Text = "Create Account";
        this.Size = new Size(420, 560);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.BackColor = Color.White;
        this.Font = new Font("Segoe UI", 10F);

        // --- Title ---
        this.lblTitle.Text = "Create New Account";
        this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        this.lblTitle.ForeColor = Color.FromArgb(30, 80, 160);
        this.lblTitle.Location = new Point(20, 20);
        this.lblTitle.Size = new Size(360, 30);

        // --- Full Name ---
        this.lblFullNameLabel.Text = "Full Name";
        this.lblFullNameLabel.Location = new Point(20, 70);
        this.lblFullNameLabel.Size = new Size(360, 20);
        this.txtFullName.Location = new Point(20, 92);
        this.txtFullName.Size = new Size(360, 28);
        this.txtFullName.BorderStyle = BorderStyle.FixedSingle;

        // --- Username ---
        this.lblUsernameLabel.Text = "Username";
        this.lblUsernameLabel.Location = new Point(20, 130);
        this.lblUsernameLabel.Size = new Size(360, 20);
        this.txtUsername.Location = new Point(20, 152);
        this.txtUsername.Size = new Size(360, 28);
        this.txtUsername.BorderStyle = BorderStyle.FixedSingle;

        // --- Email ---
        this.lblEmailLabel.Text = "Email";
        this.lblEmailLabel.Location = new Point(20, 190);
        this.lblEmailLabel.Size = new Size(360, 20);
        this.txtEmail.Location = new Point(20, 212);
        this.txtEmail.Size = new Size(360, 28);
        this.txtEmail.BorderStyle = BorderStyle.FixedSingle;

        // --- Password ---
        this.lblPasswordLabel.Text = "Password (min 6 characters)";
        this.lblPasswordLabel.Location = new Point(20, 250);
        this.lblPasswordLabel.Size = new Size(360, 20);
        this.txtPassword.Location = new Point(20, 272);
        this.txtPassword.Size = new Size(360, 28);
        this.txtPassword.BorderStyle = BorderStyle.FixedSingle;
        this.txtPassword.UseSystemPasswordChar = true;

        // --- Confirm Password ---
        this.lblConfirmPasswordLabel.Text = "Confirm Password";
        this.lblConfirmPasswordLabel.Location = new Point(20, 310);
        this.lblConfirmPasswordLabel.Size = new Size(360, 20);
        this.txtConfirmPassword.Location = new Point(20, 332);
        this.txtConfirmPassword.Size = new Size(360, 28);
        this.txtConfirmPassword.BorderStyle = BorderStyle.FixedSingle;
        this.txtConfirmPassword.UseSystemPasswordChar = true;

        // --- Role ---
        this.lblRoleLabel.Text = "Role";
        this.lblRoleLabel.Location = new Point(20, 370);
        this.lblRoleLabel.Size = new Size(360, 20);
        this.cmbRole.Location = new Point(20, 392);
        this.cmbRole.Size = new Size(360, 28);
        this.cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbRole.Items.AddRange(new object[] { "Patient", "Doctor", "Nurse", "Admin" });
        this.cmbRole.SelectedIndex = 0;

        // --- Error Label ---
        this.lblError.ForeColor = Color.FromArgb(200, 50, 50);
        this.lblError.Font = new Font("Segoe UI", 9F);
        this.lblError.Location = new Point(20, 430);
        this.lblError.Size = new Size(360, 20);
        this.lblError.Visible = false;

        // --- Register Button ---
        this.btnRegister.Text = "Create Account";
        this.btnRegister.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.btnRegister.BackColor = Color.FromArgb(30, 80, 160);
        this.btnRegister.ForeColor = Color.White;
        this.btnRegister.FlatStyle = FlatStyle.Flat;
        this.btnRegister.FlatAppearance.BorderSize = 0;
        this.btnRegister.Location = new Point(20, 460);
        this.btnRegister.Size = new Size(174, 38);
        this.btnRegister.Cursor = Cursors.Hand;
        this.btnRegister.Click += new EventHandler(this.btnRegister_Click);

        // --- Cancel Button ---
        this.btnCancel.Text = "Cancel";
        this.btnCancel.Font = new Font("Segoe UI", 10F);
        this.btnCancel.Location = new Point(206, 460);
        this.btnCancel.Size = new Size(174, 38);
        this.btnCancel.Cursor = Cursors.Hand;
        this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

        this.Controls.AddRange(new Control[]
        {
            lblTitle,
            lblFullNameLabel, txtFullName,
            lblUsernameLabel, txtUsername,
            lblEmailLabel, txtEmail,
            lblPasswordLabel, txtPassword,
            lblConfirmPasswordLabel, txtConfirmPassword,
            lblRoleLabel, cmbRole,
            lblError, btnRegister, btnCancel
        });

        this.ResumeLayout(false);
    }

    private Label lblTitle;
    private Label lblFullNameLabel;
    private TextBox txtFullName;
    private Label lblUsernameLabel;
    private TextBox txtUsername;
    private Label lblEmailLabel;
    private TextBox txtEmail;
    private Label lblPasswordLabel;
    private TextBox txtPassword;
    private Label lblConfirmPasswordLabel;
    private TextBox txtConfirmPassword;
    private Label lblRoleLabel;
    private ComboBox cmbRole;
    private Button btnRegister;
    private Button btnCancel;
    private Label lblError;
}
