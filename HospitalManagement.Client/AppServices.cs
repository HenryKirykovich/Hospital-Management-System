using HospitalManagement.Client.Services;

namespace HospitalManagement.Client;

/// <summary>
/// Holds singleton application services shared across all forms.
/// Initialized after login, disposed on logout.
/// </summary>
public static class AppServices
{
    public static SignalRService SignalR { get; private set; } = new();

    /// <summary>Called after login — starts the SignalR connection</summary>
    public static async Task InitializeAsync()
    {
        SignalR = new SignalRService();
        try
        {
            await SignalR.ConnectAsync();
        }
        catch
        {
            // SignalR connection failure is non-fatal — app works without real-time updates
        }
    }

    /// <summary>Called on logout — disposes the SignalR connection</summary>
    public static async Task DisposeAsync()
    {
        await SignalR.DisposeAsync();
    }
}
