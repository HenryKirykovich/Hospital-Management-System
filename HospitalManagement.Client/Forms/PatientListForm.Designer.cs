namespace HospitalManagement.Client.Forms;

partial class PatientListForm
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
        this.txtSearch = new TextBox();
        this.lblSearchIcon = new Label();
        this.btnRefresh = new Button();
        this.btnAdd = new Button();
        this.pnlToolbar = new Panel();
        this.btnEdit = new Button();
        this.btnDelete = new Button();
        this.btnHistory = new Button();
        this.lblCount = new Label();
        this.lblStatus = new Label();
        this.dgvPatients = new DataGridView();
        this.searchTimer = new System.Windows.Forms.Timer(this.components);
        this.pnlTop.SuspendLayout();
        this.pnlToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.dgvPatients).BeginInit();
        this.SuspendLayout();

        // --- Form ---
        this.Text = "Patient Management";
        this.Size = new Size(1100, 680);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.FromArgb(240, 244, 248);
        this.Font = new Font("Segoe UI", 10F);

        // --- Top Panel ---
        this.pnlTop.BackColor = Color.White;
        this.pnlTop.Dock = DockStyle.Top;
        this.pnlTop.Height = 64;
        this.pnlTop.Padding = new Padding(16, 12, 16, 12);

        this.lblTitle.Text = "Patient Management";
        this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        this.lblTitle.ForeColor = Color.FromArgb(30, 80, 160);
        this.lblTitle.Location = new Point(16, 16);
        this.lblTitle.Size = new Size(280, 32);

        this.txtSearch.PlaceholderText = "Search by name, email, phone...";
        this.txtSearch.Location = new Point(320, 18);
        this.txtSearch.Size = new Size(320, 28);
        this.txtSearch.BorderStyle = BorderStyle.FixedSingle;
        this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);

        this.btnRefresh.Text = "↺ Refresh";
        this.btnRefresh.Location = new Point(656, 18);
        this.btnRefresh.Size = new Size(90, 28);
        this.btnRefresh.FlatStyle = FlatStyle.Flat;
        this.btnRefresh.Cursor = Cursors.Hand;
        this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);

        this.btnAdd.Text = "+ Add Patient";
        this.btnAdd.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.btnAdd.BackColor = Color.FromArgb(30, 160, 80);
        this.btnAdd.ForeColor = Color.White;
        this.btnAdd.FlatStyle = FlatStyle.Flat;
        this.btnAdd.FlatAppearance.BorderSize = 0;
        this.btnAdd.Location = new Point(960, 16);
        this.btnAdd.Size = new Size(120, 32);
        this.btnAdd.Cursor = Cursors.Hand;
        this.btnAdd.Click += new EventHandler(this.btnAdd_Click);

        this.pnlTop.Controls.AddRange(new Control[] { lblTitle, txtSearch, btnRefresh, btnAdd });

        // --- Toolbar Panel ---
        this.pnlToolbar.BackColor = Color.FromArgb(248, 249, 252);
        this.pnlToolbar.Dock = DockStyle.Top;
        this.pnlToolbar.Height = 44;

        this.btnEdit.Text = "✏ Edit";
        this.btnEdit.Location = new Point(16, 8);
        this.btnEdit.Size = new Size(90, 28);
        this.btnEdit.FlatStyle = FlatStyle.Flat;
        this.btnEdit.Enabled = false;
        this.btnEdit.Cursor = Cursors.Hand;
        this.btnEdit.Click += new EventHandler(this.btnEdit_Click);

        this.btnDelete.Text = "✕ Delete";
        this.btnDelete.Location = new Point(116, 8);
        this.btnDelete.Size = new Size(90, 28);
        this.btnDelete.FlatStyle = FlatStyle.Flat;
        this.btnDelete.ForeColor = Color.FromArgb(180, 40, 40);
        this.btnDelete.Enabled = false;
        this.btnDelete.Cursor = Cursors.Hand;
        this.btnDelete.Click += new EventHandler(this.btnDelete_Click);

        this.btnHistory.Text = "📋 Medical History";
        this.btnHistory.Location = new Point(216, 8);
        this.btnHistory.Size = new Size(150, 28);
        this.btnHistory.FlatStyle = FlatStyle.Flat;
        this.btnHistory.Enabled = false;
        this.btnHistory.Cursor = Cursors.Hand;
        this.btnHistory.Click += new EventHandler(this.btnHistory_Click);

        this.lblCount.ForeColor = Color.Gray;
        this.lblCount.Font = new Font("Segoe UI", 9F);
        this.lblCount.Location = new Point(380, 12);
        this.lblCount.Size = new Size(200, 20);

        this.lblStatus.Text = "Loading...";
        this.lblStatus.ForeColor = Color.Gray;
        this.lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        this.lblStatus.Location = new Point(600, 12);
        this.lblStatus.Size = new Size(120, 20);
        this.lblStatus.Visible = false;

        this.pnlToolbar.Controls.AddRange(new Control[]
            { btnEdit, btnDelete, btnHistory, lblCount, lblStatus });

        // --- DataGridView ---
        this.dgvPatients.Dock = DockStyle.Fill;
        this.dgvPatients.AllowUserToAddRows = false;
        this.dgvPatients.AllowUserToDeleteRows = false;
        this.dgvPatients.ReadOnly = true;
        this.dgvPatients.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvPatients.MultiSelect = false;
        this.dgvPatients.RowHeadersVisible = false;
        this.dgvPatients.BackgroundColor = Color.White;
        this.dgvPatients.BorderStyle = BorderStyle.None;
        this.dgvPatients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvPatients.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        this.dgvPatients.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 80, 160);
        this.dgvPatients.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        this.dgvPatients.ColumnHeadersHeight = 36;
        this.dgvPatients.RowTemplate.Height = 32;
        this.dgvPatients.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
        this.dgvPatients.SelectionChanged += new EventHandler(this.dgvPatients_SelectionChanged);
        this.dgvPatients.CellDoubleClick += new DataGridViewCellEventHandler(this.dgvPatients_CellDoubleClick);

        // --- Search Timer (debounce 400ms) ---
        this.searchTimer.Interval = 400;
        this.searchTimer.Tick += new EventHandler(this.searchTimer_Tick);

        // --- Assemble ---
        this.Controls.Add(dgvPatients);
        this.Controls.Add(pnlToolbar);
        this.Controls.Add(pnlTop);

        this.pnlTop.ResumeLayout(false);
        this.pnlToolbar.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.dgvPatients).EndInit();
        this.ResumeLayout(false);
    }

    private Panel pnlTop;
    private Label lblTitle;
    private TextBox txtSearch;
    private Label lblSearchIcon;
    private Button btnRefresh;
    private Button btnAdd;
    private Panel pnlToolbar;
    private Button btnEdit;
    private Button btnDelete;
    private Button btnHistory;
    private Label lblCount;
    private Label lblStatus;
    private DataGridView dgvPatients;
    private System.Windows.Forms.Timer searchTimer;
}
