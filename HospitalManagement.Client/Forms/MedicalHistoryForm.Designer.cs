namespace HospitalManagement.Client.Forms;

partial class MedicalHistoryForm
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
        this.lblPatientName = new Label();
        this.dgvHistory = new DataGridView();
        this.pnlAddRecord = new Panel();
        this.lblAddTitle = new Label();
        this.lblDiagnosis = new Label();
        this.txtDiagnosis = new TextBox();
        this.lblTreatment = new Label();
        this.txtTreatment = new TextBox();
        this.lblNotes = new Label();
        this.txtNotes = new TextBox();
        this.btnAddRecord = new Button();
        this.btnClose = new Button();
        ((System.ComponentModel.ISupportInitialize)this.dgvHistory).BeginInit();
        this.pnlAddRecord.SuspendLayout();
        this.SuspendLayout();

        // --- Form ---
        this.Text = "Medical History";
        this.Size = new Size(900, 680);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.White;
        this.Font = new Font("Segoe UI", 10F);

        // --- Patient name label ---
        this.lblPatientName.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        this.lblPatientName.ForeColor = Color.FromArgb(30, 80, 160);
        this.lblPatientName.Location = new Point(16, 14);
        this.lblPatientName.Size = new Size(700, 28);

        // --- History Grid ---
        this.dgvHistory.Location = new Point(16, 50);
        this.dgvHistory.Size = new Size(860, 280);
        this.dgvHistory.AllowUserToAddRows = false;
        this.dgvHistory.ReadOnly = true;
        this.dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvHistory.RowHeadersVisible = false;
        this.dgvHistory.BackgroundColor = Color.White;
        this.dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        this.dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 80, 160);
        this.dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        this.dgvHistory.ColumnHeadersHeight = 34;
        this.dgvHistory.RowTemplate.Height = 30;

        // --- Add Record Panel (Doctors/Admins only) ---
        this.pnlAddRecord.Location = new Point(16, 345);
        this.pnlAddRecord.Size = new Size(860, 260);
        this.pnlAddRecord.BackColor = Color.FromArgb(245, 248, 255);
        this.pnlAddRecord.BorderStyle = BorderStyle.FixedSingle;

        this.lblAddTitle.Text = "Add New Medical Record";
        this.lblAddTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        this.lblAddTitle.ForeColor = Color.FromArgb(30, 80, 160);
        this.lblAddTitle.Location = new Point(12, 10);
        this.lblAddTitle.Size = new Size(300, 24);

        this.lblDiagnosis.Text = "Diagnosis *";
        this.lblDiagnosis.Location = new Point(12, 42);
        this.lblDiagnosis.Size = new Size(120, 20);
        this.txtDiagnosis.Location = new Point(12, 62);
        this.txtDiagnosis.Size = new Size(400, 28);
        this.txtDiagnosis.BorderStyle = BorderStyle.FixedSingle;

        this.lblTreatment.Text = "Treatment";
        this.lblTreatment.Location = new Point(12, 100);
        this.lblTreatment.Size = new Size(120, 20);
        this.txtTreatment.Location = new Point(12, 120);
        this.txtTreatment.Size = new Size(400, 28);
        this.txtTreatment.BorderStyle = BorderStyle.FixedSingle;

        this.lblNotes.Text = "Notes";
        this.lblNotes.Location = new Point(12, 158);
        this.lblNotes.Size = new Size(120, 20);
        this.txtNotes.Location = new Point(12, 178);
        this.txtNotes.Size = new Size(820, 36);
        this.txtNotes.BorderStyle = BorderStyle.FixedSingle;
        this.txtNotes.Multiline = true;

        this.btnAddRecord.Text = "Add Record";
        this.btnAddRecord.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.btnAddRecord.BackColor = Color.FromArgb(30, 80, 160);
        this.btnAddRecord.ForeColor = Color.White;
        this.btnAddRecord.FlatStyle = FlatStyle.Flat;
        this.btnAddRecord.FlatAppearance.BorderSize = 0;
        this.btnAddRecord.Location = new Point(12, 224);
        this.btnAddRecord.Size = new Size(130, 30);
        this.btnAddRecord.Cursor = Cursors.Hand;
        this.btnAddRecord.Click += new EventHandler(this.btnAddRecord_Click);

        this.pnlAddRecord.Controls.AddRange(new Control[]
        {
            lblAddTitle, lblDiagnosis, txtDiagnosis,
            lblTreatment, txtTreatment,
            lblNotes, txtNotes, btnAddRecord
        });

        // --- Close Button ---
        this.btnClose.Text = "Close";
        this.btnClose.Location = new Point(760, 615);
        this.btnClose.Size = new Size(100, 32);
        this.btnClose.FlatStyle = FlatStyle.Flat;
        this.btnClose.Cursor = Cursors.Hand;
        this.btnClose.Click += new EventHandler(this.btnClose_Click);

        this.Controls.AddRange(new Control[]
        {
            lblPatientName, dgvHistory, pnlAddRecord, btnClose
        });

        ((System.ComponentModel.ISupportInitialize)this.dgvHistory).EndInit();
        this.pnlAddRecord.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    private Label lblPatientName;
    private DataGridView dgvHistory;
    private Panel pnlAddRecord;
    private Label lblAddTitle;
    private Label lblDiagnosis;
    private TextBox txtDiagnosis;
    private Label lblTreatment;
    private TextBox txtTreatment;
    private Label lblNotes;
    private TextBox txtNotes;
    private Button btnAddRecord;
    private Button btnClose;
}
