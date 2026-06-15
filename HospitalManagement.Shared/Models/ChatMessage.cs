using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HospitalManagement.Shared.Models;

/// <summary>
/// Represents a chat message exchanged between staff and patients.
/// Used by the SignalR real-time chat module.
/// </summary>
public class ChatMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("senderId")]
    public string SenderId { get; set; } = string.Empty;

    [BsonElement("senderName")]
    public string SenderName { get; set; } = string.Empty;

    [BsonElement("receiverId")]
    public string? ReceiverId { get; set; }

    /// <summary>
    /// Channel identifier: "general" | "emergency" | patientId for private chats
    /// </summary>
    [BsonElement("channel")]
    public string Channel { get; set; } = "general";

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("sentAt")]
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    [BsonElement("isRead")]
    public bool IsRead { get; set; } = false;
}
