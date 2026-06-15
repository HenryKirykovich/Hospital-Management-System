using HospitalManagement.Client.Api;
using HospitalManagement.Shared.Models;
using System.Drawing;
using System.Windows.Forms;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Add / Edit form for an inventory item.
/// If the item parameter is null, a new item will be created (POST).
/// Otherwise the existing item will be updated (PUT).
/// </summary>
public class InventoryEditForm : Form
{
    private readonly InventoryItem? _existing;

    // Controls
    private TextBox txtName = null!;
    private ComboBox cmbCategory = null!;
    private TextBox txtDescription = null!;
    private NumericUpDown nudQuantity = null!;
    private TextBox txtUnit = null!;
    private NumericUpDown nudThreshold = null!;
    private NumericUpDown nudPrice = null!;
    private TextBox txtSupplier = null!;
    private DateTimePicker dtpExpiry = null!;
    private CheckBox chkHasExpiry = null!;
    private Button btnSave = null!;
    private Button btnCancel = null!;
    private Label lblError = null!;

    public InventoryEditForm(InventoryItem? existing)
    {
        _existing = existing;
        InitializeComponent();

        if (existing is not null)
            FillFields(existing);
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.Text = _existing is null ? "Add Inventory Item" : "Edit Inventory Item";
        this.Size = new Size(480, 540);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = Color.White;

        int labelX = 20, fieldX = 160, rowH = 38, startY = 20, fieldW = 280;

        // Helper to create a standard label
        Label MakeLabel(string text, int y) => new Label
        {
            Text = text,
            Location = new Point(labelX, y + 3),
            Size = new Size(130, 24),
            Font = new Font("Segoe UI", 10)
        };

        int row = 0;

        // Name
        int y = startY + row++ * rowH;
        this.Controls.Add(MakeLabel("Name *", y));
        txtName = new TextBox { Location = new Point(fieldX, y), Size = new Size(fieldW, 28), Font = new Font("Segoe UI", 10) };
        this.Controls.Add(txtName);

        // Category
        y = startY + row++ * rowH;
        this.Controls.Add(MakeLabel("Category *", y));
        cmbCategory = new ComboBox
        {
            Location = new Point(fieldX, y),
            Size = new Size(fieldW, 28),
            Font = new Font("Segoe UI", 10),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbCategory.Items.AddRange(new string[] { "Medication", "Supply", "Equipment" });
        cmbCategory.SelectedIndex = 0;
        this.Controls.Add(cmbCategory);

        // Description
        y = startY + row++ * rowH;
        this.Controls.Add(MakeLabel("Description", y));
        txtDescription = new TextBox { Location = new Point(fieldX, y), Size = new Size(fieldW, 28), Font = new Font("Segoe UI", 10) };
        this.Controls.Add(txtDescription);

        // Quantity
        y = startY + row++ * rowH;
        this.Controls.Add(MakeLabel("Quantity *", y));
        nudQuantity = new NumericUpDown
        {
            Location = new Point(fieldX, y),
            Size = new Size(120, 28),
            Font = new Font("Segoe UI", 10),
            Minimum = 0,
            Maximum = 999999,
            Value = 0
        };
        this.Controls.Add(nudQuantity);

        // Unit
        this.Controls.Add(new Label
        {
            Text = "Unit",
            Location = new Point(fieldX + 130, y + 3),
            Size = new Size(40, 24),
            Font = new Font("Segoe UI", 10)
        });
        txtUnit = new TextBox
        {
            Location = new Point(fieldX + 175, y),
            Size = new Size(105, 28),
            Font = new Font("Segoe UI", 10),
            Text = "units"
        };
        this.Controls.Add(txtUnit);

        // Low-stock threshold
        y = startY + row++ * rowH;
        this.Controls.Add(MakeLabel("Min Threshold *", y));
        nudThreshold = new NumericUpDown
        {
            Location = new Point(fieldX, y),
            Size = new Size(120, 28),
            Font = new Font("Segoe UI", 10),
            Minimum = 0,
            Maximum = 999999,
            Value = 10
        };
        this.Controls.Add(nudThreshold);

        // Price
        y = startY + row++ * rowH;
        this.Controls.Add(MakeLabel("Unit Price ($)", y));
        nudPrice = new NumericUpDown
        {
            Location = new Point(fieldX, y),
            Size = new Size(120, 28),
            Font = new Font("Segoe UI", 10),
            Minimum = 0,
            Maximum = 999999,
            DecimalPlaces = 2,
            Value = 0
        };
        this.Controls.Add(nudPrice);

        // Supplier
        y = startY + row++ * rowH;
        this.Controls.Add(MakeLabel("Supplier", y));
        txtSupplier = new TextBox { Location = new Point(fieldX, y), Size = new Size(fieldW, 28), Font = new Font("Segoe UI", 10) };
        this.Controls.Add(txtSupplier);

        // Expiry Date
        y = startY + row++ * rowH;
        this.Controls.Add(MakeLabel("Expiry Date", y));
        chkHasExpiry = new CheckBox
        {
            Text = "Set expiry",
            Location = new Point(fieldX, y + 2),
            Size = new Size(90, 24),
            Font = new Font("Segoe UI", 10)
        };
        dtpExpiry = new DateTimePicker
        {
            Location = new Point(fieldX + 100, y),
            Size = new Size(160, 28),
            Font = new Font("Segoe UI", 10),
            Format = DateTimePickerFormat.Short,
            Value = DateTime.Now.AddYears(2),
            Enabled = false
        };
        chkHasExpiry.CheckedChanged += (s, e) => dtpExpiry.Enabled = chkHasExpiry.Checked;
        this.Controls.Add(chkHasExpiry);
        this.Controls.Add(dtpExpiry);

        // Error label
        y = startY + row++ * rowH;
        lblError = new Label
        {
            Location = new Point(labelX, y),
            Size = new Size(430, 24),
            ForeColor = Color.Red,
            Font = new Font("Segoe UI", 9)
        };
        this.Controls.Add(lblError);

        // Buttons
        y = startY + row++ * rowH;
        btnSave = new Button
        {
            Text = _existing is null ? "Create" : "Save",
            Location = new Point(fieldX, y),
            Size = new Size(100, 34),
            BackColor = Color.FromArgb(40, 167, 69),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Click += BtnSave_Click;

        btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(fieldX + 115, y),
            Size = new Size(90, 34),
            BackColor = Color.FromArgb(108, 117, 125),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10)
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        this.Controls.Add(btnSave);
        this.Controls.Add(btnCancel);

        this.ClientSize = new Size(460, y + 70);
        this.ResumeLayout(false);
    }

    private void FillFields(InventoryItem item)
    {
        txtName.Text = item.Name;
        int catIndex = cmbCategory.Items.IndexOf(item.Category);
        cmbCategory.SelectedIndex = catIndex >= 0 ? catIndex : 0;
        txtDescription.Text = item.Description;
        nudQuantity.Value = item.Quantity;
        txtUnit.Text = item.Unit;
        nudThreshold.Value = item.LowStockThreshold;
        nudPrice.Value = item.UnitPrice;
        txtSupplier.Text = item.Supplier;

        if (item.ExpiryDate.HasValue)
        {
            chkHasExpiry.Checked = true;
            dtpExpiry.Value = item.ExpiryDate.Value;
        }
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        lblError.Text = string.Empty;

        // Validation
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            lblError.Text = "Name is required.";
            return;
        }

        var item = new InventoryItem
        {
            Name = txtName.Text.Trim(),
            Category = cmbCategory.SelectedItem?.ToString() ?? "Medication",
            Description = txtDescription.Text.Trim(),
            Quantity = (int)nudQuantity.Value,
            Unit = string.IsNullOrWhiteSpace(txtUnit.Text) ? "units" : txtUnit.Text.Trim(),
            LowStockThreshold = (int)nudThreshold.Value,
            UnitPrice = nudPrice.Value,
            Supplier = txtSupplier.Text.Trim(),
            ExpiryDate = chkHasExpiry.Checked ? dtpExpiry.Value : null
        };

        btnSave.Enabled = false;
        try
        {
            if (_existing is null)
            {
                await ApiClient.PostAsync<InventoryItem>("/api/inventory", item);
            }
            else
            {
                item.Id = _existing.Id;
                await ApiClient.PutAsync($"/api/inventory/{_existing.Id}", item);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            btnSave.Enabled = true;
        }
    }
}
