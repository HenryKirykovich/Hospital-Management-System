namespace HospitalManagement.Client.Forms;

partial class AppointmentListForm
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
        this.components = new System.ComponentModel.Container();
        this.pnlTop = new Panel();
        this.lblTitle = new Label();
        this.btnRefresh = new Button();
        this.lblNotification = new Label();
        this.pnlToolbar = new Panel();
        this.btnAdd = new Button();
        this.btnEdit = new Button();
        this.btnDelete = new Button();
        this.btnChangeStatus = new Button();
        this.lblCount = new Label();
        this.lblStatus = new Label();
        this.dgvAppointments = new DataGridView();
        this.notificationTimer = new System.Windows.Forms.Timer(this.components);
        this.pnlTop.SuspendLayout();
        this.pnlToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.dgvAppointments).BeginInit();
        this.SuspendLayout();

        // --- Form ---
        this.Text = "Appointment Scheduling";
        this.Size = new Size(1100, 680);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.FromArgb(240, 244, 248);
        this.Font = new Font("Segoe UI", 10F);

        // --- Top Panel ---
        this.pnlTop.BackColor = Color.White;
        this.pnlTop.Dock = DockStyle.Top;
        this.pnlTop.Height = 56;

        this.lblTitle.Text = "Appointment Scheduling";
        this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        this.lblTitle.ForeColor = Color.FromArgb(30, 80, 160);
        this.lblTitle.Location = new Point(16, 14);
        this.lblTitle.Size = new Size(340, 30);

        this.btnRefresh.Text = "↺ Refresh";
        this.btnRefresh.Location = new Point(370, 16);
        this.btnRefresh.Size = new Size(90, 26);
        this.btnRefresh.FlatStyle = FlatStyle.Flat;
        this.btnRefresh.Cursor = Cursors.Hand;
        this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);

        this.lblNotification.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        this.lblNotification.ForeColor = Color.FromArgb(20, 120, 20);
        this.lblNotification.Location = new Point(480, 18);
        this.lblNotification.Size = new Size(500, 22);
        this.lblNotification.Visible = false;

        this.pnlTop.Controls.AddRange(new Control[] { lblTitle, btnRefresh, lblNotification });

        // --- Toolbar ---
        this.pnlToolbar.BackColor = Color.FromArgb(248, 249, 252);
        this.pnlToolbar.Dock = DockStyle.Top;
        this.pnlToolbar.Height = 44;

        this.btnAdd.Text = "+ Add";
        this.btnAdd.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.btnAdd.BackColor = Color.FromArgb(30, 160, 80);
        this.btnAdd.ForeColor = Color.White;
        this.btnAdd.FlatStyle = FlatStyle.Flat;
        this.btnAdd.FlatAppearance.BorderSize = 0;
        this.btnAdd.Location = new Point(16, 8);
        this.btnAdd.Size = new Size(90, 28);
        this.btnAdd.Cursor = Cursors.Hand;
        this.btnAdd.Click += new EventHandler(this.btnAdd_Click);

        this.btnEdit.Text = "✏ Edit";
        this.btnEdit.Location = new Point(116, 8);
        this.btnEdit.Size = new Size(80, 28);
        this.btnEdit.FlatStyle = FlatStyle.Flat;
        this.btnEdit.Enabled = false;
        this.btnEdit.Cursor = Cursors.Hand;
        this.btnEdit.Click += new EventHandler(this.btnEdit_Click);

        this.btnDelete.Text = "✕ Delete";
        this.btnDelete.Location = new Point(206, 8);
        this.btnDelete.Size = new Size(80, 28);
        this.btnDelete.FlatStyle = FlatStyle.Flat;
        this.btnDelete.ForeColor = Color.FromArgb(180, 40, 40);
        this.btnDelete.Enabled = false;
        this.btnDelete.Cursor = Cursors.Hand;
        this.btnDelete.Click += new EventHandler(this.btnDelete_Click);

        this.btnChangeStatus.Text = "⚡ Change Status";
        this.btnChangeStatus.Location = new Point(296, 8);
        this.btnChangeStatus.Size = new Size(140, 28);
        this.btnChangeStatus.FlatStyle = FlatStyle.Flat;
        this.btnChangeStatus.BackColor = Color.FromArgb(30, 80, 160);
        this.btnChangeStatus.ForeColor = Color.White;
        this.btnChangeStatus.FlatAppearance.BorderSize = 0;
        this.btnChangeStatus.Enabled = false;
        this.btnChangeStatus.Cursor = Cursors.Hand;
        this.btnChangeStatus.Click += new EventHandler(this.btnChangeStatus_Click);

        this.lblCount.ForeColor = Color.Gray;
        this.lblCount.Font = new Font("Segoe UI", 9F);
        this.lblCount.Location = new Point(450, 13);
        this.lblCount.Size = new Size(200, 20);

        this.lblStatus.Text = "Loading...";
        this.lblStatus.ForeColor = Color.Gray;
        this.lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        this.lblStatus.Location = new Point(660, 13);
        this.lblStatus.Size = new Size(100, 20);
        this.lblStatus.Visible = false;

        this.pnlToolbar.Controls.AddRange(new Control[]
            { btnAdd, btnEdit, btnDelete, btnChangeStatus, lblCount, lblStatus });

        // --- DataGridView ---
        this.dgvAppointments.Dock = DockStyle.Fill;
        this.dgvAppointments.AllowUserToAddRows = false;
        this.dgvAppointments.AllowUserToDeleteRows = false;
        this.dgvAppointments.ReadOnly = true;
        this.dgvAppointments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvAppointments.MultiSelect = false;
        this.dgvAppointments.RowHeadersVisible = false;
        this.dgvAppointments.BackgroundColor = Color.White;
        this.dgvAppointments.BorderStyle = BorderStyle.None;
        this.dgvAppointments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvAppointments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        this.dgvAppointments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 80, 160);
        this.dgvAppointments.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        this.dgvAppointments.ColumnHeadersHeight = 36;
        this.dgvAppointments.RowTemplate.Height = 32;
        this.dgvAppointments.SelectionChanged += new EventHandler(this.dgvAppointments_SelectionChanged);
        this.dgvAppointments.CellDoubleClick += new DataGridViewCellEventHandler(this.dgvAppointments_CellDoubleClick);

        // --- Notification timer (auto-hide after 4 seconds) ---
        this.notificationTimer.Interval = 4000;
        this.notificationTimer.Tick += new EventHandler(this.notificationTimer_Tick);

        // --- Assemble ---
        this.Controls.Add(dgvAppointments);
        this.Controls.Add(pnlToolbar);
        this.Controls.Add(pnlTop);

        this.pnlTop.ResumeLayout(false);
        this.pnlToolbar.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.dgvAppointments).EndInit();
        this.ResumeLayout(false);
    }

    private Panel pnlTop;
    private Label lblTitle;
    private Button btnRefresh;
    private Label lblNotification;
    private Panel pnlToolbar;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDelete;
    private Button btnChangeStatus;
    private Label lblCount;
    private Label lblStatus;
    private DataGridView dgvAppointments;
    private System.Windows.Forms.Timer notificationTimer;
}
