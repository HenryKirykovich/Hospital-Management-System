using HospitalManagement.Client.Api;
using HospitalManagement.Client.Services;
using HospitalManagement.Client.Session;
using HospitalManagement.Shared.Models;
using System.Drawing;
using System.Windows.Forms;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Displays all inventory items in a DataGridView.
/// Low-stock rows are highlighted red.
/// Staff can add, edit, delete, or restock items.
/// SignalR keeps the grid updated in real time.
/// </summary>
public class InventoryListForm : Form
{
    // --- Controls ---
    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private System.Windows.Forms.Timer searchTimer = null!;
    private Button btnAdd = null!;
    private Button btnEdit = null!;
    private Button btnDelete = null!;
    private Button btnRestock = null!;
    private Button btnLowStock = null!;
    private DataGridView dgvInventory = null!;

    // Notification bar
    private Panel pnlNotification = null!;
    private Label lblNotification = null!;
    private System.Windows.Forms.Timer notificationTimer = null!;

    // --- State ---
    private List<InventoryItem> _items = new();
    private bool _showingLowStockOnly = false;

    public InventoryListForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.Text = "Medical Inventory";
        this.BackColor = Color.White;
        this.Padding = new Padding(0);

        // --- Notification bar (hidden by default) ---
        pnlNotification = new Panel
        {
            Dock = DockStyle.Top,
            Height = 36,
            BackColor = Color.FromArgb(220, 53, 69),  // Bootstrap danger red
            Visible = false
        };
        lblNotification = new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlNotification.Controls.Add(lblNotification);

        notificationTimer = new System.Windows.Forms.Timer { Interval = 5000 };
        notificationTimer.Tick += (s, e) => { pnlNotification.Visible = false; notificationTimer.Stop(); };

        // --- Toolbar ---
        pnlToolbar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(248, 249, 250),
            Padding = new Padding(8, 8, 8, 8)
        };

        txtSearch = new TextBox
        {
            Width = 250,
            Height = 30,
            Location = new Point(8, 10),
            Font = new Font("Segoe UI", 10),
            PlaceholderText = "Search name, category, supplier…"
        };

        searchTimer = new System.Windows.Forms.Timer { Interval = 400 };
        searchTimer.Tick += async (s, e) => { searchTimer.Stop(); await LoadInventoryAsync(txtSearch.Text); };
        txtSearch.TextChanged += (s, e) => { searchTimer.Stop(); searchTimer.Start(); };

        btnLowStock = new Button
        {
            Text = "⚠ Low Stock",
            Width = 100,
            Height = 30,
            Location = new Point(268, 10),
            BackColor = Color.FromArgb(255, 193, 7),
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        btnLowStock.FlatAppearance.BorderSize = 0;
        btnLowStock.Click += BtnLowStock_Click;

        btnAdd = new Button
        {
            Text = "+ Add Item",
            Width = 90,
            Height = 30,
            Location = new Point(378, 10),
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9)
        };
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.Click += BtnAdd_Click;

        btnEdit = new Button
        {
            Text = "✏ Edit",
            Width = 75,
            Height = 30,
            Location = new Point(478, 10),
            BackColor = Color.FromArgb(0, 123, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9),
            Enabled = false
        };
        btnEdit.FlatAppearance.BorderSize = 0;
        btnEdit.Click += BtnEdit_Click;

        btnDelete = new Button
        {
            Text = "🗑 Delete",
            Width = 80,
            Height = 30,
            Location = new Point(563, 10),
            BackColor = Color.FromArgb(220, 53, 69),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9),
            Enabled = false
        };
        btnDelete.FlatAppearance.BorderSize = 0;
        btnDelete.Click += BtnDelete_Click;

        btnRestock = new Button
        {
            Text = "📦 Restock",
            Width = 90,
            Height = 30,
            Location = new Point(653, 10),
            BackColor = Color.FromArgb(23, 162, 184),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9),
            Enabled = false
        };
        btnRestock.FlatAppearance.BorderSize = 0;
        btnRestock.Click += BtnRestock_Click;

        pnlToolbar.Controls.AddRange(new Control[] { txtSearch, btnLowStock, btnAdd, btnEdit, btnDelete, btnRestock });

        // --- DataGridView ---
        dgvInventory = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            ColumnHeadersHeight = 40,
            Font = new Font("Segoe UI", 10),
            GridColor = Color.FromArgb(222, 226, 230),
            DefaultCellStyle = new DataGridViewCellStyle
            {
                SelectionBackColor = Color.FromArgb(0, 123, 255),
                SelectionForeColor = Color.White,
                Padding = new Padding(4)
            },
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Padding = new Padding(4)
            }
        };
        dgvInventory.EnableHeadersVisualStyles = false;

        // Columns
        dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "ID", Visible = false });
        dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colName", HeaderText = "Name", FillWeight = 200 });
        dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCategory", HeaderText = "Category", FillWeight = 100 });
        dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colQuantity", HeaderText = "Qty", FillWeight = 60 });
        dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUnit", HeaderText = "Unit", FillWeight = 60 });
        dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colThreshold", HeaderText = "Min Threshold", FillWeight = 80 });
        dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Status", FillWeight = 80 });
        dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPrice", HeaderText = "Unit Price", FillWeight = 80 });
        dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSupplier", HeaderText = "Supplier", FillWeight = 150 });
        dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { Name = "colExpiry", HeaderText = "Expiry Date", FillWeight = 90 });

        dgvInventory.SelectionChanged += DgvInventory_SelectionChanged;
        dgvInventory.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) BtnEdit_Click(s, e); };

        // Layout: notification bar on top, then toolbar, then grid
        this.Controls.Add(dgvInventory);
        this.Controls.Add(pnlToolbar);
        this.Controls.Add(pnlNotification);

        this.ResumeLayout(false);
    }

    // ====== Lifecycle ======

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Role-based permissions
        bool isStaff = ClientSession.IsStaff;
        btnAdd.Enabled = isStaff;

        // Subscribe to SignalR events
        if (AppServices.SignalR is not null)
        {
            AppServices.SignalR.OnInventoryUpdated += SignalR_InventoryUpdated;
            AppServices.SignalR.OnInventoryDeleted += SignalR_InventoryDeleted;
            AppServices.SignalR.OnLowStockAlert += SignalR_LowStockAlert;
        }

        await LoadInventoryAsync();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
        searchTimer.Stop();
        notificationTimer.Stop();

        if (AppServices.SignalR is not null)
        {
            AppServices.SignalR.OnInventoryUpdated -= SignalR_InventoryUpdated;
            AppServices.SignalR.OnInventoryDeleted -= SignalR_InventoryDeleted;
            AppServices.SignalR.OnLowStockAlert -= SignalR_LowStockAlert;
        }
    }

    // ====== Data Loading ======

    private async Task LoadInventoryAsync(string? search = null)
    {
        SetBusy(true);
        try
        {
            string endpoint = string.IsNullOrWhiteSpace(search)
                ? (_showingLowStockOnly ? "/api/inventory/low-stock" : "/api/inventory")
                : $"/api/inventory/search?q={Uri.EscapeDataString(search)}";

            var result = await ApiClient.GetAsync<List<InventoryItem>>(endpoint);
            _items = result ?? new List<InventoryItem>();
            PopulateGrid(_items);
        }
        catch (Exception ex)
        {
            ShowNotification($"Error loading inventory: {ex.Message}", isAlert: false);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void PopulateGrid(List<InventoryItem> items)
    {
        dgvInventory.Rows.Clear();

        foreach (var item in items)
        {
            string status = item.IsLowStock ? "⚠ Low Stock" : "✓ In Stock";
            string price = item.UnitPrice > 0 ? $"${item.UnitPrice:F2}" : "—";
            string expiry = item.ExpiryDate.HasValue ? item.ExpiryDate.Value.ToString("yyyy-MM-dd") : "—";

            int rowIndex = dgvInventory.Rows.Add(
                item.Id,
                item.Name,
                item.Category,
                item.Quantity,
                item.Unit,
                item.LowStockThreshold,
                status,
                price,
                item.Supplier,
                expiry
            );

            // Highlight low-stock rows
            if (item.IsLowStock)
            {
                var row = dgvInventory.Rows[rowIndex];
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(183, 28, 28);
            }
        }

        UpdateTitle();
    }

    private void UpdateTitle()
    {
        int lowCount = _items.Count(i => i.IsLowStock);
        string title = _showingLowStockOnly
            ? $"Medical Inventory — Low Stock ({_items.Count} items)"
            : $"Medical Inventory — {_items.Count} items";

        if (lowCount > 0 && !_showingLowStockOnly)
            title += $"  ⚠ {lowCount} low stock";

        this.Text = title;
    }

    // ====== Toolbar Handlers ======

    private async void BtnLowStock_Click(object? sender, EventArgs e)
    {
        _showingLowStockOnly = !_showingLowStockOnly;
        btnLowStock.BackColor = _showingLowStockOnly
            ? Color.FromArgb(220, 53, 69)
            : Color.FromArgb(255, 193, 7);
        btnLowStock.ForeColor = _showingLowStockOnly ? Color.White : Color.Black;
        btnLowStock.Text = _showingLowStockOnly ? "⚠ All Items" : "⚠ Low Stock";
        txtSearch.Text = string.Empty;
        await LoadInventoryAsync();
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var form = new InventoryEditForm(null);
        if (form.ShowDialog(this) == DialogResult.OK)
            _ = LoadInventoryAsync();
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        var selected = GetSelectedItem();
        if (selected is null) return;

        using var form = new InventoryEditForm(selected);
        if (form.ShowDialog(this) == DialogResult.OK)
            _ = LoadInventoryAsync();
    }

    private async void BtnDelete_Click(object? sender, EventArgs e)
    {
        var selected = GetSelectedItem();
        if (selected is null) return;

        var confirm = MessageBox.Show(
            $"Delete '{selected.Name}'?\nThis action cannot be undone.",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes) return;

        try
        {
            await ApiClient.DeleteAsync($"/api/inventory/{selected.Id}");
            await LoadInventoryAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Delete failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnRestock_Click(object? sender, EventArgs e)
    {
        var selected = GetSelectedItem();
        if (selected is null) return;

        using var dlg = new RestockDialog(selected.Name, selected.Quantity, selected.Unit);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            await ApiClient.PatchAsync($"/api/inventory/{selected.Id}/restock",
                new { Quantity = dlg.RestockAmount });
            await LoadInventoryAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Restock failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ====== SignalR Handlers ======

    private void SignalR_InventoryUpdated(InventoryItem item)
    {
        InvokeOnUiThread(() =>
        {
            // Update row if visible, else reload
            bool found = false;
            foreach (DataGridViewRow row in dgvInventory.Rows)
            {
                if (row.Cells["colId"].Value?.ToString() == item.Id)
                {
                    row.Cells["colName"].Value = item.Name;
                    row.Cells["colCategory"].Value = item.Category;
                    row.Cells["colQuantity"].Value = item.Quantity;
                    row.Cells["colUnit"].Value = item.Unit;
                    row.Cells["colThreshold"].Value = item.LowStockThreshold;
                    row.Cells["colStatus"].Value = item.IsLowStock ? "⚠ Low Stock" : "✓ In Stock";
                    row.Cells["colPrice"].Value = item.UnitPrice > 0 ? $"${item.UnitPrice:F2}" : "—";
                    row.Cells["colSupplier"].Value = item.Supplier;
                    row.Cells["colExpiry"].Value = item.ExpiryDate.HasValue ? item.ExpiryDate.Value.ToString("yyyy-MM-dd") : "—";

                    // Re-apply colour
                    row.DefaultCellStyle.BackColor = item.IsLowStock
                        ? Color.FromArgb(255, 235, 238)
                        : Color.White;
                    row.DefaultCellStyle.ForeColor = item.IsLowStock
                        ? Color.FromArgb(183, 28, 28)
                        : Color.Black;

                    found = true;
                    break;
                }
            }

            if (!found)
                _ = LoadInventoryAsync();
        });
    }

    private void SignalR_InventoryDeleted(string itemId)
    {
        InvokeOnUiThread(() =>
        {
            foreach (DataGridViewRow row in dgvInventory.Rows)
            {
                if (row.Cells["colId"].Value?.ToString() == itemId)
                {
                    dgvInventory.Rows.Remove(row);
                    break;
                }
            }
        });
    }

    private void SignalR_LowStockAlert(dynamic alert)
    {
        InvokeOnUiThread(() =>
        {
            try
            {
                string name = alert?.name?.ToString() ?? "Unknown item";
                int qty = (int)(alert?.quantity ?? 0);
                int threshold = (int)(alert?.threshold ?? 0);
                ShowNotification($"⚠ LOW STOCK: {name} — {qty} units remaining (threshold: {threshold})", isAlert: true);
            }
            catch
            {
                ShowNotification("⚠ Low stock alert received", isAlert: true);
            }
        });
    }

    // ====== Helpers ======

    private void DgvInventory_SelectionChanged(object? sender, EventArgs e)
    {
        bool hasRow = dgvInventory.SelectedRows.Count > 0;
        bool isAdmin = ClientSession.IsAdmin;
        bool isStaff = ClientSession.IsStaff;

        btnEdit.Enabled = hasRow && isStaff;
        btnDelete.Enabled = hasRow && isAdmin;
        btnRestock.Enabled = hasRow && isStaff;
    }

    private InventoryItem? GetSelectedItem()
    {
        if (dgvInventory.SelectedRows.Count == 0) return null;
        var row = dgvInventory.SelectedRows[0];
        string? id = row.Cells["colId"].Value?.ToString();
        return _items.FirstOrDefault(i => i.Id == id);
    }

    private void SetBusy(bool busy)
    {
        btnAdd.Enabled = !busy && ClientSession.IsStaff;
        txtSearch.Enabled = !busy;
        btnLowStock.Enabled = !busy;

        if (!busy)
            DgvInventory_SelectionChanged(null, EventArgs.Empty);
    }

    private void ShowNotification(string message, bool isAlert)
    {
        lblNotification.Text = message;
        pnlNotification.BackColor = isAlert
            ? Color.FromArgb(220, 53, 69)
            : Color.FromArgb(23, 162, 184);
        pnlNotification.Visible = true;
        notificationTimer.Stop();
        notificationTimer.Start();
    }

    private void InvokeOnUiThread(Action action)
    {
        if (this.IsDisposed || !this.IsHandleCreated) return;
        if (this.InvokeRequired)
            this.Invoke(action);
        else
            action();
    }
}
