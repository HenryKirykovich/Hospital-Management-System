namespace HospitalManagement.Client.Forms;

partial class PatientEditForm
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
        this.lblFirstName = new Label();
        this.txtFirstName = new TextBox();
        this.lblLastName = new Label();
        this.txtLastName = new TextBox();
        this.lblDob = new Label();
        this.dtpDob = new DateTimePicker();
        this.lblGender = new Label();
        this.cmbGender = new ComboBox();
        this.lblPhone = new Label();
        this.txtPhone = new TextBox();
        this.lblEmail = new Label();
        this.txtEmail = new TextBox();
        this.lblAddress = new Label();
        this.txtAddress = new TextBox();
        this.lblBloodType = new Label();
        this.cmbBloodType = new ComboBox();
        this.lblAllergies = new Label();
        this.txtAllergies = new TextBox();
        this.lblError = new Label();
        this.btnSave = new Button();
        this.btnCancel = new Button();
        this.SuspendLayout();

        // --- Form ---
        this.Text = "Patient";
        this.Size = new Size(520, 600);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.BackColor = Color.White;
        this.Font = new Font("Segoe UI", 10F);

        // Title
        this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        this.lblTitle.ForeColor = Color.FromArgb(30, 80, 160);
        this.lblTitle.Location = new Point(20, 16);
        this.lblTitle.Size = new Size(460, 32);

        // Helper to create a label + textbox row
        int y = 60;
        int lw = 140, tw = 320, lh = 20, th = 28, gap = 12;

        void AddRow(Label lbl, string labelText, Control ctrl)
        {
            lbl.Text = labelText;
            lbl.Location = new Point(20, y);
            lbl.Size = new Size(lw, lh);
            lbl.ForeColor = Color.FromArgb(60, 60, 60);
            ctrl.Location = new Point(20, y + lh + 2);
            ctrl.Size = new Size(tw, th);
            if (ctrl is TextBox tb) tb.BorderStyle = BorderStyle.FixedSingle;
            y += lh + th + gap + 4;
        }

        AddRow(lblFirstName, "First Name *", txtFirstName);
        AddRow(lblLastName, "Last Name *", txtLastName);

        // Date of Birth
        lblDob.Text = "Date of Birth *";
        lblDob.Location = new Point(20, y);
        lblDob.Size = new Size(lw, lh);
        lblDob.ForeColor = Color.FromArgb(60, 60, 60);
        dtpDob.Location = new Point(20, y + lh + 2);
        dtpDob.Size = new Size(tw, th);
        dtpDob.Format = DateTimePickerFormat.Short;
        dtpDob.MaxDate = DateTime.Today.AddDays(-1);
        dtpDob.Value = DateTime.Today.AddYears(-30);
        y += lh + th + gap + 4;

        // Gender
        lblGender.Text = "Gender";
        lblGender.Location = new Point(20, y);
        lblGender.Size = new Size(lw, lh);
        lblGender.ForeColor = Color.FromArgb(60, 60, 60);
        cmbGender.Location = new Point(20, y + lh + 2);
        cmbGender.Size = new Size(tw, th);
        cmbGender.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbGender.Items.AddRange(new object[] { "Male", "Female", "Other", "Prefer not to say" });
        cmbGender.SelectedIndex = 0;
        y += lh + th + gap + 4;

        AddRow(lblPhone, "Phone", txtPhone);
        AddRow(lblEmail, "Email", txtEmail);
        AddRow(lblAddress, "Address", txtAddress);

        // Blood Type
        lblBloodType.Text = "Blood Type";
        lblBloodType.Location = new Point(20, y);
        lblBloodType.Size = new Size(lw, lh);
        lblBloodType.ForeColor = Color.FromArgb(60, 60, 60);
        cmbBloodType.Location = new Point(20, y + lh + 2);
        cmbBloodType.Size = new Size(tw, th);
        cmbBloodType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbBloodType.Items.AddRange(new object[] { "", "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });
        cmbBloodType.SelectedIndex = 0;
        y += lh + th + gap + 4;

        AddRow(lblAllergies, "Allergies (comma-separated)", txtAllergies);

        // Error label
        lblError.ForeColor = Color.FromArgb(200, 50, 50);
        lblError.Font = new Font("Segoe UI", 9F);
        lblError.Location = new Point(20, y);
        lblError.Size = new Size(460, 20);
        lblError.Visible = false;
        y += 28;

        // Buttons
        btnSave.Text = "Save";
        btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnSave.BackColor = Color.FromArgb(30, 80, 160);
        btnSave.ForeColor = Color.White;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Location = new Point(20, y);
        btnSave.Size = new Size(160, 36);
        btnSave.Cursor = Cursors.Hand;
        btnSave.Click += new EventHandler(this.btnSave_Click);

        btnCancel.Text = "Cancel";
        btnCancel.Location = new Point(192, y);
        btnCancel.Size = new Size(100, 36);
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.Cursor = Cursors.Hand;
        btnCancel.Click += new EventHandler(this.btnCancel_Click);

        // Adjust form height
        this.ClientSize = new Size(480, y + 60);

        this.Controls.AddRange(new Control[]
        {
            lblTitle,
            lblFirstName, txtFirstName,
            lblLastName, txtLastName,
            lblDob, dtpDob,
            lblGender, cmbGender,
            lblPhone, txtPhone,
            lblEmail, txtEmail,
            lblAddress, txtAddress,
            lblBloodType, cmbBloodType,
            lblAllergies, txtAllergies,
            lblError, btnSave, btnCancel
        });

        this.ResumeLayout(false);
    }

    private Label lblTitle;
    private Label lblFirstName;
    private TextBox txtFirstName;
    private Label lblLastName;
    private TextBox txtLastName;
    private Label lblDob;
    private DateTimePicker dtpDob;
    private Label lblGender;
    private ComboBox cmbGender;
    private Label lblPhone;
    private TextBox txtPhone;
    private Label lblEmail;
    private TextBox txtEmail;
    private Label lblAddress;
    private TextBox txtAddress;
    private Label lblBloodType;
    private ComboBox cmbBloodType;
    private Label lblAllergies;
    private TextBox txtAllergies;
    private Label lblError;
    private Button btnSave;
    private Button btnCancel;
}
