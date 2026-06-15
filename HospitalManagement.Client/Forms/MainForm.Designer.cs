namespace HospitalManagement.Client.Forms;

partial class MainForm
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
        this.pnlHeader = new Panel();
        this.lblAppTitle = new Label();
        this.lblWelcome = new Label();
        this.btnLogout = new Button();
        this.pnlSidebar = new Panel();
        this.lblMenuTitle = new Label();
        this.pnlContent = new Panel();
        this.pnlHeader.SuspendLayout();
        this.pnlSidebar.SuspendLayout();
        this.pnlContent.SuspendLayout();
        this.SuspendLayout();

        // --- Form ---
        this.Text = "Hospital Management System";
        this.Size = new Size(1200, 750);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(900, 600);
        this.BackColor = Color.FromArgb(240, 244, 248);
        this.Font = new Font("Segoe UI", 10F);

        // --- Header Panel ---
        this.pnlHeader.BackColor = Color.FromArgb(30, 80, 160);
        this.pnlHeader.Dock = DockStyle.Top;
        this.pnlHeader.Height = 56;
        this.pnlHeader.Padding = new Padding(16, 0, 16, 0);

        // App title in header
        this.lblAppTitle.Text = "Hospital Management System";
        this.lblAppTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        this.lblAppTitle.ForeColor = Color.White;
        this.lblAppTitle.Location = new Point(16, 12);
        this.lblAppTitle.Size = new Size(400, 32);
        this.lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;

        // Welcome label in header
        this.lblWelcome.Font = new Font("Segoe UI", 9F);
        this.lblWelcome.ForeColor = Color.FromArgb(200, 220, 255);
        this.lblWelcome.Location = new Point(430, 18);
        this.lblWelcome.Size = new Size(580, 20);
        this.lblWelcome.TextAlign = ContentAlignment.MiddleLeft;

        // Logout button in header
        this.btnLogout.Text = "Logout";
        this.btnLogout.Font = new Font("Segoe UI", 9F);
        this.btnLogout.BackColor = Color.FromArgb(200, 50, 50);
        this.btnLogout.ForeColor = Color.White;
        this.btnLogout.FlatStyle = FlatStyle.Flat;
        this.btnLogout.FlatAppearance.BorderSize = 0;
        this.btnLogout.Size = new Size(80, 30);
        this.btnLogout.Location = new Point(1080, 13);
        this.btnLogout.Cursor = Cursors.Hand;
        this.btnLogout.Click += new EventHandler(this.btnLogout_Click);

        this.pnlHeader.Controls.AddRange(new Control[] { lblAppTitle, lblWelcome, btnLogout });

        // --- Sidebar Panel ---
        this.pnlSidebar.BackColor = Color.FromArgb(22, 55, 120);
        this.pnlSidebar.Dock = DockStyle.Left;
        this.pnlSidebar.Width = 220;

        this.lblMenuTitle.Text = "NAVIGATION";
        this.lblMenuTitle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        this.lblMenuTitle.ForeColor = Color.FromArgb(150, 180, 230);
        this.lblMenuTitle.Location = new Point(16, 20);
        this.lblMenuTitle.Size = new Size(190, 20);

        this.pnlSidebar.Controls.Add(lblMenuTitle);

        // --- Content Panel ---
        this.pnlContent.BackColor = Color.FromArgb(240, 244, 248);
        this.pnlContent.Dock = DockStyle.Fill;
        this.pnlContent.Padding = new Padding(20);

        this.pnlContent.Controls.Clear();  // content loaded dynamically via sidebar

        // --- Assemble ---
        this.Controls.Add(pnlContent);
        this.Controls.Add(pnlSidebar);
        this.Controls.Add(pnlHeader);

        this.pnlHeader.ResumeLayout(false);
        this.pnlSidebar.ResumeLayout(false);
        this.pnlContent.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    private Panel pnlHeader;
    private Label lblAppTitle;
    private Label lblWelcome;
    private Button btnLogout;
    private Panel pnlSidebar;
    private Label lblMenuTitle;
    private Panel pnlContent;
}
