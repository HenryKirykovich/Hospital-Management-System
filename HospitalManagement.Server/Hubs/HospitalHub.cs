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

    public HospitalHub(ILogger<HospitalHub> logger)
    {
        _logger = logger;
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
}
