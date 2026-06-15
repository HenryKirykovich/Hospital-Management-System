using HospitalManagement.Server.Data;
using HospitalManagement.Shared.Models;
using MongoDB.Driver;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Handles all inventory CRUD and stock operations against MongoDB.
/// Low-stock detection is done after each quantity change — the controller
/// broadcasts a SignalR alert when IsLowStock becomes true.
/// </summary>
public class InventoryService : IInventoryService
{
    private readonly MongoDbContext _db;

    public InventoryService(MongoDbContext db)
    {
        _db = db;
    }

    public async Task<List<InventoryItem>> GetAllAsync()
    {
        return await _db.Inventory
            .Find(_ => true)
            .SortBy(i => i.Category)
            .ThenBy(i => i.Name)
            .ToListAsync();
    }

    /// <summary>Returns only items where quantity is at or below the low-stock threshold</summary>
    public async Task<List<InventoryItem>> GetLowStockAsync()
    {
        var filter = Builders<InventoryItem>.Filter
            .Where(i => i.Quantity <= i.LowStockThreshold);

        return await _db.Inventory
            .Find(filter)
            .SortBy(i => i.Quantity)
            .ToListAsync();
    }

    /// <summary>Case-insensitive search across name, category, and supplier</summary>
    public async Task<List<InventoryItem>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetAllAsync();

        var regex = new MongoDB.Bson.BsonRegularExpression(query, "i");
        var filter = Builders<InventoryItem>.Filter.Or(
            Builders<InventoryItem>.Filter.Regex(i => i.Name, regex),
            Builders<InventoryItem>.Filter.Regex(i => i.Category, regex),
            Builders<InventoryItem>.Filter.Regex(i => i.Supplier, regex));

        return await _db.Inventory
            .Find(filter)
            .SortBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<InventoryItem?> GetByIdAsync(string id)
    {
        return await _db.Inventory
            .Find(i => i.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<InventoryItem> CreateAsync(InventoryItem item)
    {
        item.CreatedAt = DateTime.UtcNow;
        await _db.Inventory.InsertOneAsync(item);
        return item;
    }

    public async Task<bool> UpdateAsync(string id, InventoryItem item)
    {
        item.Id = id;
        var result = await _db.Inventory.ReplaceOneAsync(i => i.Id == id, item);
        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// Adds the specified quantity to current stock and records restock timestamp.
    /// Returns the updated item so the caller can check if it is still low-stock.
    /// </summary>
    public async Task<bool> RestockAsync(string id, int quantity)
    {
        var update = Builders<InventoryItem>.Update
            .Inc(i => i.Quantity, quantity)
            .Set(i => i.LastRestockedAt, DateTime.UtcNow);

        var result = await _db.Inventory.UpdateOneAsync(i => i.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _db.Inventory.DeleteOneAsync(i => i.Id == id);
        return result.DeletedCount > 0;
    }
}
