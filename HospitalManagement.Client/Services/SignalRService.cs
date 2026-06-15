using HospitalManagement.Client.Session;
using Microsoft.AspNetCore.SignalR.Client;

namespace HospitalManagement.Client.Services;

/// <summary>
/// Manages the SignalR connection to the server hub.
/// Singleton — created once after login, disposed on logout.
/// Raises events that WinForms forms can subscribe to for real-time UI updates.
/// </summary>
public class SignalRService : IAsyncDisposable
{
    private HubConnection? _connection;

    // --- Events raised when the server broadcasts messages ---
    public event Action<object>? OnAppointmentCreated;
    public event Action<object>? OnAppointmentUpdated;
    public event Action<object>? OnAppointmentStatusChanged;
    public event Action<string>? OnAppointmentDeleted;
    public event Action<object>? OnInventoryUpdated;
    public event Action<object>? OnLowStockAlert;
    public event Action<object>? OnChatMessage;
    public event Action<object>? OnVitalsUpdate;
    public event Action<string>? OnNotification;

    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    /// <summary>
    /// Connects to the SignalR hub using the stored JWT token for authentication.
    /// Call this once after a successful login.
    /// </summary>
    public async Task ConnectAsync()
    {
        var hubUrl = $"{Api.ApiClient.BaseUrl}/hubs/hospital";

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                // Pass JWT token via query string (configured in server JwtBearer events)
                options.AccessTokenProvider = () =>
                    Task.FromResult<string?>(ClientSession.Token);
            })
            .WithAutomaticReconnect()
            .Build();

        RegisterHandlers();

        await _connection.StartAsync();
    }

    /// <summary>
    /// Registers all incoming message handlers from the server hub.
    /// </summary>
    private void RegisterHandlers()
    {
        if (_connection is null) return;

        _connection.On<object>("AppointmentCreated",
            data => OnAppointmentCreated?.Invoke(data));

        _connection.On<object>("AppointmentUpdated",
            data => OnAppointmentUpdated?.Invoke(data));

        _connection.On<object>("AppointmentStatusChanged",
            data => OnAppointmentStatusChanged?.Invoke(data));

        _connection.On<string>("AppointmentDeleted",
            id => OnAppointmentDeleted?.Invoke(id));

        _connection.On<object>("InventoryUpdated",
            data => OnInventoryUpdated?.Invoke(data));

        _connection.On<object>("LowStockAlert",
            data => OnLowStockAlert?.Invoke(data));

        _connection.On<object>("ChatMessage",
            data => OnChatMessage?.Invoke(data));

        _connection.On<object>("VitalsUpdate",
            data => OnVitalsUpdate?.Invoke(data));

        _connection.On<string>("Notification",
            msg => OnNotification?.Invoke(msg));

        _connection.Reconnected += connectionId =>
        {
            OnNotification?.Invoke("Reconnected to server.");
            return Task.CompletedTask;
        };

        _connection.Reconnecting += error =>
        {
            OnNotification?.Invoke("Connection lost. Reconnecting...");
            return Task.CompletedTask;
        };
    }

    /// <summary>Joins a named group on the hub (e.g. a patient room or chat channel)</summary>
    public async Task JoinGroupAsync(string groupName)
    {
        if (_connection is not null && IsConnected)
            await _connection.InvokeAsync("JoinGroup", groupName);
    }

    /// <summary>Leaves a named group on the hub</summary>
    public async Task LeaveGroupAsync(string groupName)
    {
        if (_connection is not null && IsConnected)
            await _connection.InvokeAsync("LeaveGroup", groupName);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}
