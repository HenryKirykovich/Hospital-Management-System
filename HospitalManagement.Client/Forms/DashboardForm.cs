using HospitalManagement.Client.Api;
using HospitalManagement.Shared.DTOs;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Analytics dashboard with stat cards and GDI+ charts.
/// Shows: overview counters, appointments by status (bar), 
/// monthly trend (line), and inventory by category (horizontal bar).
/// </summary>
public class DashboardForm : Form
{
    private Panel pnlMain = null!;
    private Button btnRefresh = null!;
    private Label lblLastUpdated = null!;

    // Stat card panels
    private Panel pnlPatients = null!;
    private Panel pnlStaff = null!;
    private Panel pnlApptToday = null!;
    private Panel pnlApptMonth = null!;
    private Panel pnlPending = null!;
    private Panel pnlLowStock = null!;

    // Chart panels
    private Panel pnlBarChart = null!;
    private Panel pnlLineChart = null!;
    private Panel pnlInventoryChart = null!;

    private DashboardData? _data;

    public DashboardForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.Text = "Dashboard";
        this.BackColor = Color.FromArgb(245, 247, 250);
        this.AutoScroll = true;

        // --- Top bar ---
        var pnlTopBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(248, 249, 250),
            Padding = new Padding(16, 10, 16, 10)
        };

        var lblTitle = new Label
        {
            Text = "Analytics Dashboard",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 37, 41),
            AutoSize = true,
            Location = new Point(16, 12)
        };

        btnRefresh = new Button
        {
            Text = "↻ Refresh",
            Location = new Point(320, 10),
            Size = new Size(90, 30),
            BackColor = Color.FromArgb(0, 123, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9)
        };
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Click += async (s, e) => await LoadDataAsync();

        lblLastUpdated = new Label
        {
            Text = "",
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.Gray,
            AutoSize = true,
            Location = new Point(420, 16)
        };

        pnlTopBar.Controls.AddRange(new Control[] { lblTitle, btnRefresh, lblLastUpdated });

        // --- Scrollable main area ---
        pnlMain = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.FromArgb(245, 247, 250),
            Padding = new Padding(16)
        };

        // --- Stat cards row ---
        pnlPatients  = MakeCard("👤 Patients",         "—", Color.FromArgb(0, 123, 255),   new Point(16, 16));
        pnlStaff     = MakeCard("👨‍⚕️ Staff",            "—", Color.FromArgb(40, 167, 69),   new Point(156, 16));
        pnlApptToday = MakeCard("📅 Appts Today",      "—", Color.FromArgb(23, 162, 184),  new Point(296, 16));
        pnlApptMonth = MakeCard("📆 Appts This Month", "—", Color.FromArgb(111, 66, 193),  new Point(436, 16));
        pnlPending   = MakeCard("⏳ Pending",           "—", Color.FromArgb(255, 193, 7),   new Point(576, 16));
        pnlLowStock  = MakeCard("⚠ Low Stock",         "—", Color.FromArgb(220, 53, 69),   new Point(716, 16));

        // --- Chart panels ---
        pnlBarChart = MakeChartPanel("Appointments by Status", new Point(16, 146), new Size(420, 240));
        pnlLineChart = MakeChartPanel("Monthly Appointments (Last 6 Months)", new Point(452, 146), new Size(420, 240));
        pnlInventoryChart = MakeChartPanel("Inventory by Category", new Point(16, 402), new Size(856, 200));

        pnlMain.Controls.AddRange(new Control[]
        {
            pnlPatients, pnlStaff, pnlApptToday, pnlApptMonth, pnlPending, pnlLowStock,
            pnlBarChart, pnlLineChart, pnlInventoryChart
        });

        this.Controls.Add(pnlMain);
        this.Controls.Add(pnlTopBar);

        this.ResumeLayout(false);
    }

    // ====== Lifecycle ======

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await LoadDataAsync();
    }

    // ====== Data ======

    private async Task LoadDataAsync()
    {
        btnRefresh.Enabled = false;
        try
        {
            _data = await ApiClient.GetAsync<DashboardData>("/api/analytics/dashboard");
            if (_data is not null)
                UpdateUI(_data);
            lblLastUpdated.Text = $"Updated {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            lblLastUpdated.Text = $"Error: {ex.Message}";
        }
        finally
        {
            btnRefresh.Enabled = true;
        }
    }

    private void UpdateUI(DashboardData d)
    {
        SetCardValue(pnlPatients,  d.Overview.TotalPatients.ToString());
        SetCardValue(pnlStaff,     d.Overview.TotalStaff.ToString());
        SetCardValue(pnlApptToday, d.Overview.AppointmentsToday.ToString());
        SetCardValue(pnlApptMonth, d.Overview.AppointmentsThisMonth.ToString());
        SetCardValue(pnlPending,   d.Overview.AppointmentsPending.ToString());
        SetCardValue(pnlLowStock,  d.Overview.LowStockItems.ToString());

        // Redraw charts
        pnlBarChart.Invalidate();
        pnlLineChart.Invalidate();
        pnlInventoryChart.Invalidate();
    }

    // ====== Card Builders ======

    private static Panel MakeCard(string title, string value, Color accent, Point location)
    {
        var card = new Panel
        {
            Location = location,
            Size = new Size(130, 110),
            BackColor = Color.White,
            Cursor = Cursors.Default
        };
        card.Paint += (s, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Accent top bar
            using var accentBrush = new SolidBrush(accent);
            g.FillRectangle(accentBrush, 0, 0, card.Width, 5);

            // Shadow border
            using var borderPen = new Pen(Color.FromArgb(220, 220, 220));
            g.DrawRectangle(borderPen, 0, 0, card.Width - 1, card.Height - 1);
        };

        var lblTitle = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = Color.Gray,
            Location = new Point(10, 14),
            Size = new Size(110, 32),
            TextAlign = ContentAlignment.TopLeft,
            AutoEllipsis = true
        };

        var lblValue = new Label
        {
            Text = value,
            Name = "lblValue",
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = accent,
            Location = new Point(10, 48),
            Size = new Size(110, 50),
            TextAlign = ContentAlignment.MiddleLeft
        };

        card.Controls.AddRange(new Control[] { lblTitle, lblValue });
        return card;
    }

    private static void SetCardValue(Panel card, string value)
    {
        var lbl = card.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "lblValue");
        if (lbl is not null) lbl.Text = value;
    }

    // ====== Chart Panel Builder ======

    private Panel MakeChartPanel(string title, Point location, Size size)
    {
        var panel = new Panel
        {
            Location = location,
            Size = size,
            BackColor = Color.White
        };
        panel.Paint += (s, e) => DrawChart(e.Graphics, panel, title);

        // Attach paint event to chart panels that need data
        return panel;
    }

    private void DrawChart(Graphics g, Panel panel, string title)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        // Background & border
        g.FillRectangle(Brushes.White, 0, 0, panel.Width, panel.Height);
        using var borderPen = new Pen(Color.FromArgb(220, 220, 220));
        g.DrawRectangle(borderPen, 0, 0, panel.Width - 1, panel.Height - 1);

        // Title
        using var titleFont = new Font("Segoe UI", 9, FontStyle.Bold);
        g.DrawString(title, titleFont, Brushes.DimGray, new PointF(12, 10));

        if (_data is null)
        {
            g.DrawString("Loading…", new Font("Segoe UI", 10), Brushes.LightGray,
                new PointF(panel.Width / 2f - 30, panel.Height / 2f));
            return;
        }

        if (title.StartsWith("Appointments by Status"))
            DrawStatusBarChart(g, panel);
        else if (title.StartsWith("Monthly"))
            DrawMonthlyLineChart(g, panel);
        else if (title.StartsWith("Inventory"))
            DrawInventoryBarChart(g, panel);
    }

    // ====== Appointments by Status — Vertical Bar Chart ======

    private static readonly Color[] StatusColors = new[]
    {
        Color.FromArgb(23, 162, 184),   // Scheduled
        Color.FromArgb(40, 167, 69),    // Confirmed
        Color.FromArgb(0, 123, 255),    // Completed
        Color.FromArgb(220, 53, 69),    // Cancelled
        Color.FromArgb(255, 193, 7)     // NoShow
    };

    private void DrawStatusBarChart(Graphics g, Panel panel)
    {
        var s = _data!.AppointmentsByStatus;
        var values = new[] { s.Scheduled, s.Confirmed, s.Completed, s.Cancelled, s.NoShow };
        var labels = new[] { "Scheduled", "Confirmed", "Completed", "Cancelled", "No Show" };

        int chartLeft = 40, chartTop = 30, chartBottom = panel.Height - 50;
        int chartRight = panel.Width - 16;
        int chartH = chartBottom - chartTop;
        int max = Math.Max(values.Max(), 1);
        int barCount = values.Length;
        int totalW = chartRight - chartLeft;
        int barW = totalW / barCount - 12;

        using var axisFont = new Font("Segoe UI", 7);
        using var gridPen = new Pen(Color.FromArgb(230, 230, 230));

        // Y-axis grid lines (4 lines)
        for (int i = 1; i <= 4; i++)
        {
            float yy = chartBottom - (chartH * i / 4f);
            g.DrawLine(gridPen, chartLeft, yy, chartRight, yy);
            int val = max * i / 4;
            g.DrawString(val.ToString(), axisFont, Brushes.Gray, new PointF(4, yy - 7));
        }

        // Bars
        for (int i = 0; i < barCount; i++)
        {
            float barH = values[i] == 0 ? 2 : chartH * values[i] / (float)max;
            float x = chartLeft + i * (totalW / barCount) + 6;
            float y = chartBottom - barH;

            using var brush = new SolidBrush(StatusColors[i]);
            g.FillRectangle(brush, x, y, barW, barH);

            // Value label above bar
            if (values[i] > 0)
                g.DrawString(values[i].ToString(), axisFont, Brushes.DimGray,
                    new PointF(x + barW / 2f - 5, y - 14));

            // X axis label
            var lblRect = new RectangleF(x - 4, chartBottom + 4, barW + 8, 30);
            var fmt = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(labels[i], axisFont, Brushes.DimGray, lblRect, fmt);
        }

        // X axis line
        using var axisPen = new Pen(Color.FromArgb(180, 180, 180));
        g.DrawLine(axisPen, chartLeft, chartBottom, chartRight, chartBottom);
    }

    // ====== Monthly Appointments — Line Chart ======

    private void DrawMonthlyLineChart(Graphics g, Panel panel)
    {
        var monthly = _data!.MonthlyAppointments;
        if (monthly.Count == 0) return;

        int chartLeft = 40, chartTop = 30, chartBottom = panel.Height - 50;
        int chartRight = panel.Width - 16;
        int chartH = chartBottom - chartTop;
        int chartW = chartRight - chartLeft;
        int max = Math.Max(monthly.Max(m => m.Count), 1);
        int n = monthly.Count;

        using var axisFont = new Font("Segoe UI", 7);
        using var gridPen = new Pen(Color.FromArgb(230, 230, 230));
        using var linePen = new Pen(Color.FromArgb(0, 123, 255), 2.5f);
        linePen.StartCap = LineCap.Round;
        linePen.EndCap = LineCap.Round;

        // Grid lines
        for (int i = 1; i <= 4; i++)
        {
            float yy = chartBottom - (chartH * i / 4f);
            g.DrawLine(gridPen, chartLeft, yy, chartRight, yy);
            int val = max * i / 4;
            g.DrawString(val.ToString(), axisFont, Brushes.Gray, new PointF(4, yy - 7));
        }

        // Points
        var points = new PointF[n];
        for (int i = 0; i < n; i++)
        {
            float x = chartLeft + i * (chartW / (float)(n - 1 == 0 ? 1 : n - 1));
            float y = chartBottom - chartH * monthly[i].Count / (float)max;
            points[i] = new PointF(x, y);
        }

        // Filled area under line
        if (n >= 2)
        {
            var fillPath = new GraphicsPath();
            fillPath.AddLine(points[0].X, chartBottom, points[0].X, points[0].Y);
            for (int i = 1; i < n; i++)
                fillPath.AddLine(points[i - 1], points[i]);
            fillPath.AddLine(points[n - 1].X, points[n - 1].Y, points[n - 1].X, chartBottom);
            fillPath.CloseFigure();
            using var fillBrush = new SolidBrush(Color.FromArgb(30, 0, 123, 255));
            g.FillPath(fillBrush, fillPath);

            // Line
            g.DrawLines(linePen, points);
        }

        // Dots and values
        for (int i = 0; i < n; i++)
        {
            g.FillEllipse(Brushes.White, points[i].X - 4, points[i].Y - 4, 8, 8);
            using var dotBrush = new SolidBrush(Color.FromArgb(0, 123, 255));
            g.FillEllipse(dotBrush, points[i].X - 3, points[i].Y - 3, 6, 6);

            if (monthly[i].Count > 0)
                g.DrawString(monthly[i].Count.ToString(), axisFont, Brushes.DimGray,
                    new PointF(points[i].X - 5, points[i].Y - 16));

            // Month label
            var fmt = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(monthly[i].Month.Replace(" ", "\n"), axisFont, Brushes.DimGray,
                new RectangleF(points[i].X - 28, chartBottom + 4, 56, 36), fmt);
        }

        // X axis line
        using var axisPen = new Pen(Color.FromArgb(180, 180, 180));
        g.DrawLine(axisPen, chartLeft, chartBottom, chartRight, chartBottom);
    }

    // ====== Inventory by Category — Horizontal Bar Chart ======

    private static readonly Color[] CategoryColors = new[]
    {
        Color.FromArgb(0, 123, 255),
        Color.FromArgb(40, 167, 69),
        Color.FromArgb(23, 162, 184),
        Color.FromArgb(255, 193, 7),
        Color.FromArgb(111, 66, 193)
    };

    private void DrawInventoryBarChart(Graphics g, Panel panel)
    {
        var cats = _data!.InventoryByCategory;
        if (cats.Count == 0) return;

        using var axisFont = new Font("Segoe UI", 8);
        using var boldFont = new Font("Segoe UI", 8, FontStyle.Bold);

        int labelW = 100, chartLeft = labelW + 10, chartRight = panel.Width - 120;
        int barH = 28, gap = 10, startY = 32;
        int maxQty = Math.Max(cats.Max(c => c.TotalQuantity), 1);
        int chartW = chartRight - chartLeft;

        for (int i = 0; i < cats.Count; i++)
        {
            var cat = cats[i];
            int y = startY + i * (barH + gap);
            float barLen = chartW * cat.TotalQuantity / (float)maxQty;

            // Category label
            var catRect = new RectangleF(8, y + 6, labelW - 4, barH - 4);
            var fmt = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
            g.DrawString(cat.Category, axisFont, Brushes.DimGray, catRect, fmt);

            // Background bar
            g.FillRectangle(new SolidBrush(Color.FromArgb(235, 235, 235)), chartLeft, y, chartW, barH);

            // Filled bar
            using var barBrush = new SolidBrush(CategoryColors[i % CategoryColors.Length]);
            if (barLen > 0)
                g.FillRectangle(barBrush, chartLeft, y, barLen, barH);

            // Stats label to the right
            string info = $"{cat.TotalQuantity} units  |  {cat.TotalItems} items";
            if (cat.LowStockCount > 0)
                info += $"  ⚠{cat.LowStockCount}";
            g.DrawString(info, axisFont, cat.LowStockCount > 0 ? Brushes.Firebrick : Brushes.DimGray,
                new PointF(chartRight + 8, y + 7));
        }

        // X axis line
        using var axisPen = new Pen(Color.FromArgb(180, 180, 180));
        g.DrawLine(axisPen, chartLeft, startY + cats.Count * (barH + gap), chartRight, startY + cats.Count * (barH + gap));
    }
}
