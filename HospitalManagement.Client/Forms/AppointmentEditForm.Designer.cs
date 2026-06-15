using HospitalManagement.Shared.Models;

namespace HospitalManagement.Client.Forms;

partial class AppointmentEditForm
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
        this.lblPatient = new Label();
        this.cmbPatient = new ComboBox();
        this.txtPatientName = new TextBox();
        this.lblDoctor = new Label();
        this.cmbDoctor = new ComboBox();
        this.txtDoctorName = new TextBox();
        this.lblDate = new Label();
        this.dtpDate = new DateTimePicker();
        this.lblDuration = new Label();
        this.nudDuration = new NumericUpDown();
        this.lblReason = new Label();
        this.txtReason = new TextBox();
        this.lblStatus = new Label();
        this.cmbStatus = new ComboBox();
        this.lblNotes = new Label();
        this.txtNotes = new TextBox();
        this.lblError = new Label();
        this.btnSave = new Button();
        this.btnCancel = new Button();
        ((System.ComponentModel.ISupportInitialize)this.nudDuration).BeginInit();
        this.SuspendLayout();

        this.Text = "Appointment";
        this.Size = new Size(500, 560);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.BackColor = Color.White;
        this.Font = new Font("Segoe UI", 10F);

        int y = 16; int lh = 20; int th = 28; int gap = 10;
        int lw = 130; int fw = 330;

        void Row(Label lbl, string text, Control ctrl)
        {
            lbl.Text = text; lbl.Location = new Point(20, y); lbl.Size = new Size(fw, lh);
            lbl.ForeColor = Color.FromArgb(60, 60, 60);
            ctrl.Location = new Point(20, y + lh + 2); ctrl.Size = new Size(fw, th);
            if (ctrl is TextBox tb) tb.BorderStyle = BorderStyle.FixedSingle;
            if (ctrl is ComboBox cb) cb.DropDownStyle = ComboBoxStyle.DropDownList;
            y += lh + th + gap + 2;
        }

        // Title
        this.lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        this.lblTitle.ForeColor = Color.FromArgb(30, 80, 160);
        this.lblTitle.Location = new Point(20, y); this.lblTitle.Size = new Size(fw, 28); y += 36;

        // Patient dropdown + fallback text
        this.lblPatient.Text = "Patient";
        this.lblPatient.Location = new Point(20, y); this.lblPatient.Size = new Size(fw, lh);
        this.lblPatient.ForeColor = Color.FromArgb(60, 60, 60);
        this.cmbPatient.Location = new Point(20, y + lh + 2); this.cmbPatient.Size = new Size(fw, th);
        this.cmbPatient.DropDownStyle = ComboBoxStyle.DropDownList;
        this.txtPatientName.PlaceholderText = "Or type patient name manually";
        this.txtPatientName.Location = new Point(20, y + lh + th + 6); this.txtPatientName.Size = new Size(fw, th);
        this.txtPatientName.BorderStyle = BorderStyle.FixedSingle;
        this.txtPatientName.Font = new Font("Segoe UI", 9F);
        y += lh + th + th + gap + 8;

        // Doctor dropdown + fallback text
        this.lblDoctor.Text = "Doctor";
        this.lblDoctor.Location = new Point(20, y); this.lblDoctor.Size = new Size(fw, lh);
        this.lblDoctor.ForeColor = Color.FromArgb(60, 60, 60);
        this.cmbDoctor.Location = new Point(20, y + lh + 2); this.cmbDoctor.Size = new Size(fw, th);
        this.cmbDoctor.DropDownStyle = ComboBoxStyle.DropDownList;
        this.txtDoctorName.PlaceholderText = "Or type doctor name manually";
        this.txtDoctorName.Location = new Point(20, y + lh + th + 6); this.txtDoctorName.Size = new Size(fw, th);
        this.txtDoctorName.BorderStyle = BorderStyle.FixedSingle;
        this.txtDoctorName.Font = new Font("Segoe UI", 9F);
        y += lh + th + th + gap + 8;

        // Date/time
        this.lblDate.Text = "Date & Time *";
        this.lblDate.Location = new Point(20, y); this.lblDate.Size = new Size(fw, lh);
        this.lblDate.ForeColor = Color.FromArgb(60, 60, 60);
        this.dtpDate.Location = new Point(20, y + lh + 2); this.dtpDate.Size = new Size(fw, th);
        this.dtpDate.Format = DateTimePickerFormat.Custom;
        this.dtpDate.CustomFormat = "yyyy-MM-dd  HH:mm";
        this.dtpDate.Value = DateTime.Now.AddHours(1);
        y += lh + th + gap + 2;

        // Duration
        this.lblDuration.Text = "Duration (minutes)";
        this.lblDuration.Location = new Point(20, y); this.lblDuration.Size = new Size(fw, lh);
        this.lblDuration.ForeColor = Color.FromArgb(60, 60, 60);
        this.nudDuration.Location = new Point(20, y + lh + 2); this.nudDuration.Size = new Size(100, th);
        this.nudDuration.Minimum = 5; this.nudDuration.Maximum = 240; this.nudDuration.Value = 30;
        y += lh + th + gap + 2;

        Row(lblReason, "Reason", txtReason);

        // Status
        this.lblStatus.Text = "Status";
        this.lblStatus.Location = new Point(20, y); this.lblStatus.Size = new Size(fw, lh);
        this.lblStatus.ForeColor = Color.FromArgb(60, 60, 60);
        this.cmbStatus.Location = new Point(20, y + lh + 2); this.cmbStatus.Size = new Size(fw, th);
        this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbStatus.Items.AddRange(new object[] { "Scheduled", "Confirmed", "Cancelled", "Completed", "NoShow" });
        this.cmbStatus.SelectedIndex = 0;
        y += lh + th + gap + 2;

        Row(lblNotes, "Notes", txtNotes);

        // Error
        this.lblError.ForeColor = Color.FromArgb(200, 50, 50);
        this.lblError.Font = new Font("Segoe UI", 9F);
        this.lblError.Location = new Point(20, y); this.lblError.Size = new Size(fw, 20);
        this.lblError.Visible = false; y += 26;

        // Buttons
        this.btnSave.Text = "Save"; this.btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.btnSave.BackColor = Color.FromArgb(30, 80, 160); this.btnSave.ForeColor = Color.White;
        this.btnSave.FlatStyle = FlatStyle.Flat; this.btnSave.FlatAppearance.BorderSize = 0;
        this.btnSave.Location = new Point(20, y); this.btnSave.Size = new Size(140, 34);
        this.btnSave.Cursor = Cursors.Hand;
        this.btnSave.Click += new EventHandler(this.btnSave_Click);

        this.btnCancel.Text = "Cancel";
        this.btnCancel.Location = new Point(170, y); this.btnCancel.Size = new Size(100, 34);
        this.btnCancel.FlatStyle = FlatStyle.Flat; this.btnCancel.Cursor = Cursors.Hand;
        this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

        this.ClientSize = new Size(460, y + 56);

        this.Controls.AddRange(new Control[]
        {
            lblTitle,
            lblPatient, cmbPatient, txtPatientName,
            lblDoctor, cmbDoctor, txtDoctorName,
            lblDate, dtpDate,
            lblDuration, nudDuration,
            lblReason, txtReason,
            lblStatus, cmbStatus,
            lblNotes, txtNotes,
            lblError, btnSave, btnCancel
        });

        ((System.ComponentModel.ISupportInitialize)this.nudDuration).EndInit();
        this.ResumeLayout(false);
    }

    private Label lblTitle;
    private Label lblPatient;
    private ComboBox cmbPatient;
    private TextBox txtPatientName;
    private Label lblDoctor;
    private ComboBox cmbDoctor;
    private TextBox txtDoctorName;
    private Label lblDate;
    private DateTimePicker dtpDate;
    private Label lblDuration;
    private NumericUpDown nudDuration;
    private Label lblReason;
    private TextBox txtReason;
    private Label lblStatus;
    private ComboBox cmbStatus;
    private Label lblNotes;
    private TextBox txtNotes;
    private Label lblError;
    private Button btnSave;
    private Button btnCancel;
}
