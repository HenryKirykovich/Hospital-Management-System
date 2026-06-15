namespace HospitalManagement.Client.Forms;

/// <summary>
/// Small dialog that lets the user pick a new appointment status from a list.
/// </summary>
public class StatusPickerDialog : Form
{
    public string SelectedStatus { get; private set; } = string.Empty;

    private readonly ComboBox cmbStatus = new();
    private readonly Button btnOk = new();
    private readonly Button btnCancel = new();

    public StatusPickerDialog(string currentStatus, string[] options)
    {
        Text = "Change Status";
        Size = new Size(300, 160);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 10F);

        var lbl = new Label
        {
            Text = "Select new status:",
            Location = new Point(16, 16),
            Size = new Size(260, 20)
        };

        cmbStatus.Location = new Point(16, 40);
        cmbStatus.Size = new Size(260, 28);
        cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStatus.Items.AddRange(options);
        cmbStatus.SelectedItem = currentStatus;

        btnOk.Text = "OK";
        btnOk.BackColor = Color.FromArgb(30, 80, 160);
        btnOk.ForeColor = Color.White;
        btnOk.FlatStyle = FlatStyle.Flat;
        btnOk.FlatAppearance.BorderSize = 0;
        btnOk.Location = new Point(16, 80);
        btnOk.Size = new Size(120, 30);
        btnOk.Click += (_, _) =>
        {
            SelectedStatus = cmbStatus.SelectedItem?.ToString() ?? string.Empty;
            DialogResult = DialogResult.OK;
            Close();
        };

        btnCancel.Text = "Cancel";
        btnCancel.Location = new Point(148, 80);
        btnCancel.Size = new Size(80, 30);
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.AddRange(new Control[] { lbl, cmbStatus, btnOk, btnCancel });
    }
}
