using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HospitalManagement.Shared.Models;

/// <summary>
/// Represents a medication or medical supply in the hospital inventory.
/// Low stock triggers a SignalR alert to relevant staff.
/// </summary>
public class InventoryItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Category: Medication | Supply | Equipment</summary>
    [BsonElement("category")]
    public string Category { get; set; } = "Medication";

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("unit")]
    public string Unit { get; set; } = "units";

    /// <summary>Alert is triggered when Quantity falls at or below this value</summary>
    [BsonElement("lowStockThreshold")]
    public int LowStockThreshold { get; set; } = 10;

    [BsonElement("unitPrice")]
    public decimal UnitPrice { get; set; }

    [BsonElement("supplier")]
    public string Supplier { get; set; } = string.Empty;

    [BsonElement("expiryDate")]
    public DateTime? ExpiryDate { get; set; }

    [BsonElement("lastRestockedAt")]
    public DateTime? LastRestockedAt { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Computed property — not stored in MongoDB</summary>
    [BsonIgnore]
    public bool IsLowStock => Quantity <= LowStockThreshold;
}
