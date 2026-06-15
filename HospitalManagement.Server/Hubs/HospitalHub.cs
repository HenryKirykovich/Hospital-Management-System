using HospitalManagement.Server.Services;
using HospitalManagement.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HospitalManagement.Server.Hubs;

/// <summary>
/// Central SignalR hub for all real-time features:
/// - Appointment notifications
/// - Inventory low-stock alerts
/// - Patient vitals monitoring
/// - Chat messages
/// - Dashboard updates
/// </summary>
[Authorize]
public class HospitalHub : Hub
{
    private readonly ILogger<HospitalHub> _logger;
    private readonly IChatService _chatService;

    public HospitalHub(ILogger<HospitalHub> logger, IChatService chatService)
    {
        _logger = logger;
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var role = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        // Add user to their role group so we can broadcast to specific roles
        if (!string.IsNullOrEmpty(role))
            await Groups.AddToGroupAsync(Context.ConnectionId, role);

        _logger.LogInformation("Client connected: {UserId} ({Role})", userId, role);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {UserId}", Context.UserIdentifier);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Client calls this to join a specific channel (e.g. a patient room or chat channel).
    /// </summary>
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Client calls this to leave a channel.
    /// </summary>
    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Client calls this to send a chat message to a channel.
    /// The message is persisted in MongoDB and then broadcast to all channel members.
    /// </summary>
    public async Task SendMessage(string channel, string content)
    {
        var userId   = Context.UserIdentifier ?? "unknown";
        var fullName = Context.User?.FindFirst("fullName")?.Value
                       ?? Context.User?.Identity?.Name
                       ?? "Unknown";

        var message = new ChatMessage
        {
            SenderId   = userId,
            SenderName = fullName,
            Channel    = channel,
            Content    = content
        };

        var saved = await _chatService.SaveMessageAsync(message);

        // Broadcast to everyone in the channel group
        await Clients.Group(channel).SendAsync("ChatMessage", saved);

        // Also send to emergency group if it is an emergency message
        if (channel == "emergency")
            await Clients.Groups("Admin", "Doctor", "Nurse")
                .SendAsync("Notification", $"🚨 EMERGENCY from {fullName}: {content}");
    }
}
