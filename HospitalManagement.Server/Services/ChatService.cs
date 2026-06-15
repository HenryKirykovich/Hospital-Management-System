using HospitalManagement.Server.Data;
using HospitalManagement.Shared.Models;
using MongoDB.Driver;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Persists and retrieves chat messages from MongoDB.
/// </summary>
public class ChatService : IChatService
{
    private readonly MongoDbContext _db;

    public ChatService(MongoDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns the last <paramref name="limit"/> messages for the channel, ordered oldest → newest.
    /// </summary>
    public async Task<List<ChatMessage>> GetChannelHistoryAsync(string channel, int limit = 50)
    {
        return await _db.ChatMessages
            .Find(m => m.Channel == channel)
            .SortByDescending(m => m.SentAt)
            .Limit(limit)
            .ToListAsync()
            .ContinueWith(t =>
            {
                // Reverse so UI shows oldest first (top to bottom)
                var list = t.Result;
                list.Reverse();
                return list;
            });
    }

    /// <summary>
    /// Inserts a message into MongoDB and returns it with the assigned Id.
    /// </summary>
    public async Task<ChatMessage> SaveMessageAsync(ChatMessage message)
    {
        message.SentAt = DateTime.UtcNow;
        await _db.ChatMessages.InsertOneAsync(message);
        return message;
    }

    /// <summary>
    /// Marks all unread messages in a channel not sent by the given user as read.
    /// </summary>
    public async Task MarkChannelReadAsync(string channel, string userId)
    {
        var filter = Builders<ChatMessage>.Filter.And(
            Builders<ChatMessage>.Filter.Eq(m => m.Channel, channel),
            Builders<ChatMessage>.Filter.Ne(m => m.SenderId, userId),
            Builders<ChatMessage>.Filter.Eq(m => m.IsRead, false));

        var update = Builders<ChatMessage>.Update.Set(m => m.IsRead, true);
        await _db.ChatMessages.UpdateManyAsync(filter, update);
    }
}
