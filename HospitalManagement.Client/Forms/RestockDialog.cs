using System.Drawing;
using System.Windows.Forms;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Simple dialog that asks the user how many units to add to stock.
/// Shows current stock level so staff knows the context.
/// </summary>
public class RestockDialog : Form
{
    public int RestockAmount => (int)nudAmount.Value;

    private NumericUpDown nudAmount = null!;
    private Button btnOk = null!;
    private Button btnCancel = null!;

    public RestockDialog(string itemName, int currentQty, string unit)
    {
        this.Text = "Restock Item";
        this.Size = new Size(360, 200);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = Color.White;

        var lblInfo = new Label
        {
            Text = $"Item: {itemName}\nCurrent stock: {currentQty} {unit}",
            Location = new Point(20, 20),
            Size = new Size(310, 42),
            Font = new Font("Segoe UI", 10)
        };

        var lblAdd = new Label
        {
            Text = "Add quantity:",
            Location = new Point(20, 72),
            Size = new Size(110, 26),
            Font = new Font("Segoe UI", 10)
        };

        nudAmount = new NumericUpDown
        {
            Location = new Point(140, 70),
            Size = new Size(100, 28),
            Font = new Font("Segoe UI", 10),
            Minimum = 1,
            Maximum = 9999,
            Value = 10
        };

        btnOk = new Button
        {
            Text = "Restock",
            Location = new Point(80, 115),
            Size = new Size(90, 32),
            BackColor = Color.FromArgb(23, 162, 184),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        btnOk.FlatAppearance.BorderSize = 0;
        btnOk.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };

        btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(185, 115),
            Size = new Size(80, 32),
            BackColor = Color.FromArgb(108, 117, 125),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9)
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        this.Controls.AddRange(new Control[] { lblInfo, lblAdd, nudAmount, btnOk, btnCancel });
    }
}
