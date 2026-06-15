using HospitalManagement.Shared.Models;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Contract for chat message persistence and retrieval.
/// </summary>
public interface IChatService
{
    /// <summary>Returns the most recent messages for a channel (newest last)</summary>
    Task<List<ChatMessage>> GetChannelHistoryAsync(string channel, int limit = 50);

    /// <summary>Saves a message and returns it with the generated Id</summary>
    Task<ChatMessage> SaveMessageAsync(ChatMessage message);

    /// <summary>Marks all unread messages in a channel as read for a user</summary>
    Task MarkChannelReadAsync(string channel, string userId);
}
