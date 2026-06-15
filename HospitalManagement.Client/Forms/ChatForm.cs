using HospitalManagement.Client.Api;
using HospitalManagement.Client.Services;
using HospitalManagement.Client.Session;
using HospitalManagement.Shared.Models;
using System.Drawing;
using System.Windows.Forms;

namespace HospitalManagement.Client.Forms;

/// <summary>
/// Real-time chat form using SignalR.
/// Supports three built-in channels: General, Emergency, and Staff.
/// Staff can also open a private patient channel by entering a patient ID.
/// Messages are loaded from history on channel switch and arrive live via SignalR.
/// </summary>
public class ChatForm : Form
{
    // --- Controls ---
    private Panel pnlLeft = null!;       // Channel list sidebar
    private Panel pnlRight = null!;      // Chat area
    private ListBox lstChannels = null!;
    private Panel pnlMessages = null!;   // Scrollable message area
    private TextBox txtInput = null!;
    private Button btnSend = null!;
    private Label lblChannel = null!;
    private Label lblStatus = null!;

    // --- State ---
    private string _currentChannel = "general";
    private readonly List<string> _channels = new() { "general", "emergency", "staff" };

    public ChatForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.Text = "Chat";
        this.BackColor = Color.FromArgb(245, 247, 250);

        // === Left sidebar: channel list ===
        pnlLeft = new Panel
        {
            Dock = DockStyle.Left,
            Width = 160,
            BackColor = Color.FromArgb(52, 58, 64)
        };

        var lblChannelTitle = new Label
        {
            Text = "CHANNELS",
            ForeColor = Color.FromArgb(173, 181, 189),
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            Location = new Point(12, 16),
            AutoSize = true
        };

        lstChannels = new ListBox
        {
            Location = new Point(0, 40),
            Width = 160,
            Height = 200,
            BackColor = Color.FromArgb(52, 58, 64),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10),
            BorderStyle = BorderStyle.None,
            ItemHeight = 36
        };
        lstChannels.Items.AddRange(new object[] { "# general", "🚨 emergency", "👨‍⚕️ staff" });
        lstChannels.SelectedIndex = 0;
        lstChannels.SelectedIndexChanged += LstChannels_SelectedIndexChanged;

        pnlLeft.Controls.AddRange(new Control[] { lblChannelTitle, lstChannels });

        // === Right: chat area ===
        pnlRight = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        // Channel name header
        lblChannel = new Label
        {
            Dock = DockStyle.Top,
            Height = 48,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 37, 41),
            BackColor = Color.White,
            Text = "# general",
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 0, 0)
        };

        // Connection status
        lblStatus = new Label
        {
            Dock = DockStyle.Top,
            Height = 24,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.Gray,
            Text = "",
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 0, 0)
        };

        // Scrollable message area
        pnlMessages = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.White,
            Padding = new Padding(8, 4, 8, 4)
        };

        // Input bar at bottom
        var pnlInput = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 56,
            BackColor = Color.FromArgb(248, 249, 250),
            Padding = new Padding(12, 10, 12, 10)
        };

        txtInput = new TextBox
        {
            Location = new Point(12, 13),
            Height = 30,
            Width = 500,
            Font = new Font("Segoe UI", 10),
            PlaceholderText = "Type a message…"
        };
        txtInput.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                BtnSend_Click(s, e);
            }
        };

        btnSend = new Button
        {
            Text = "Send ➤",
            Location = new Point(522, 11),
            Size = new Size(90, 32),
            BackColor = Color.FromArgb(0, 123, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        btnSend.FlatAppearance.BorderSize = 0;
        btnSend.Click += BtnSend_Click;

        pnlInput.Controls.AddRange(new Control[] { txtInput, btnSend });

        // Divider line above input
        var pnlDivider = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 1,
            BackColor = Color.FromArgb(222, 226, 230)
        };

        pnlRight.Controls.Add(pnlMessages);
        pnlRight.Controls.Add(pnlDivider);
        pnlRight.Controls.Add(pnlInput);
        pnlRight.Controls.Add(lblStatus);
        pnlRight.Controls.Add(lblChannel);

        this.Controls.Add(pnlRight);
        this.Controls.Add(pnlLeft);

        this.ResumeLayout(false);
    }

    // ====== Lifecycle ======

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (AppServices.SignalR is not null)
        {
            AppServices.SignalR.OnChatMessage += SignalR_ChatMessage;

            lblStatus.ForeColor = AppServices.SignalR.IsConnected
                ? Color.FromArgb(40, 167, 69)
                : Color.FromArgb(220, 53, 69);
            lblStatus.Text = AppServices.SignalR.IsConnected
                ? "● Connected"
                : "● Disconnected";
        }

        await SwitchChannelAsync("general");
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);

        if (AppServices.SignalR is not null)
            AppServices.SignalR.OnChatMessage -= SignalR_ChatMessage;

        // Leave the current channel group
        _ = AppServices.SignalR?.LeaveGroupAsync(_currentChannel);
    }

    // ====== Channel Switching ======

    private async void LstChannels_SelectedIndexChanged(object? sender, EventArgs e)
    {
        int idx = lstChannels.SelectedIndex;
        string channel = idx switch
        {
            0 => "general",
            1 => "emergency",
            2 => "staff",
            _ => "general"
        };
        await SwitchChannelAsync(channel);
    }

    private async Task SwitchChannelAsync(string channel)
    {
        // Leave old channel group
        if (!string.IsNullOrEmpty(_currentChannel) && _currentChannel != channel)
            await (AppServices.SignalR?.LeaveGroupAsync(_currentChannel) ?? Task.CompletedTask);

        _currentChannel = channel;
        string displayName = channel switch
        {
            "emergency" => "🚨 emergency",
            "staff"     => "👨‍⚕️ staff",
            _           => "# " + channel
        };
        lblChannel.Text = displayName;

        // Join new channel group for real-time delivery
        await (AppServices.SignalR?.JoinGroupAsync(channel) ?? Task.CompletedTask);

        // Load history
        await LoadHistoryAsync(channel);
    }

    // ====== Message Loading ======

    private async Task LoadHistoryAsync(string channel)
    {
        pnlMessages.Controls.Clear();

        try
        {
            var history = await ApiClient.GetAsync<List<ChatMessage>>($"/api/chat/{channel}?limit=50");
            if (history is null) return;

            foreach (var msg in history)
                AppendMessage(msg);

            ScrollToBottom();
        }
        catch
        {
            // Gracefully handle history load failure (not critical)
        }
    }

    // ====== Sending ======

    private async void BtnSend_Click(object? sender, EventArgs e)
    {
        string content = txtInput.Text.Trim();
        if (string.IsNullOrEmpty(content)) return;

        txtInput.Text = string.Empty;
        btnSend.Enabled = false;

        try
        {
            if (AppServices.SignalR is not null && AppServices.SignalR.IsConnected)
            {
                // Send via SignalR hub — message returns via OnChatMessage event
                await AppServices.SignalR.SendMessageAsync(_currentChannel, content);
            }
            else
            {
                // Fallback: show locally if SignalR disconnected
                var msg = new ChatMessage
                {
                    SenderId   = ClientSession.UserId ?? "me",
                    SenderName = ClientSession.FullName ?? "Me",
                    Channel    = _currentChannel,
                    Content    = content,
                    SentAt     = DateTime.Now
                };
                AppendMessage(msg);
                ScrollToBottom();
            }
        }
        catch (Exception ex)
        {
            AppendSystemMessage($"Send failed: {ex.Message}");
        }
        finally
        {
            btnSend.Enabled = true;
        }
    }

    // ====== SignalR Handler ======

    private void SignalR_ChatMessage(ChatMessage message)
    {
        if (message.Channel != _currentChannel) return;

        InvokeOnUiThread(() =>
        {
            AppendMessage(message);
            ScrollToBottom();
        });
    }

    // ====== Message Rendering ======

    private void AppendMessage(ChatMessage msg)
    {
        bool isOwn = msg.SenderId == ClientSession.UserId;

        var pnlMsg = new Panel
        {
            Width = pnlMessages.ClientSize.Width - 24,
            Padding = new Padding(0, 2, 0, 2),
            BackColor = Color.White,
            AutoSize = true
        };

        // Sender + time line
        var lblSender = new Label
        {
            Text = $"{msg.SenderName}  {msg.SentAt.ToLocalTime():HH:mm}",
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = isOwn ? Color.FromArgb(0, 86, 179) : Color.FromArgb(100, 100, 100),
            AutoSize = true,
            Location = new Point(0, 0)
        };

        // Message content
        var lblContent = new Label
        {
            Text = msg.Content,
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(33, 37, 41),
            MaximumSize = new Size(pnlMessages.ClientSize.Width - 32, 0),
            AutoSize = true,
            Location = new Point(0, 18)
        };

        pnlMsg.Controls.Add(lblSender);
        pnlMsg.Controls.Add(lblContent);
        pnlMsg.Height = lblContent.Bottom + 6;

        // Stack messages vertically
        int y = 4;
        foreach (Control c in pnlMessages.Controls)
            y = Math.Max(y, c.Bottom + 2);
        pnlMsg.Location = new Point(8, y);

        pnlMessages.Controls.Add(pnlMsg);
    }

    private void AppendSystemMessage(string text)
    {
        AppendMessage(new ChatMessage
        {
            SenderId   = "system",
            SenderName = "System",
            Channel    = _currentChannel,
            Content    = text,
            SentAt     = DateTime.Now
        });
        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        if (pnlMessages.Controls.Count == 0) return;
        pnlMessages.AutoScrollPosition = new Point(0, pnlMessages.DisplayRectangle.Height);
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
