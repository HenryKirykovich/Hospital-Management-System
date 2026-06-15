using HospitalManagement.Shared.Models;

namespace HospitalManagement.Server.Services;

/// <summary>
/// Contract for inventory CRUD and stock management operations.
/// </summary>
public interface IInventoryService
{
    Task<List<InventoryItem>> GetAllAsync();
    Task<List<InventoryItem>> GetLowStockAsync();
    Task<List<InventoryItem>> SearchAsync(string query);
    Task<InventoryItem?> GetByIdAsync(string id);
    Task<InventoryItem> CreateAsync(InventoryItem item);
    Task<bool> UpdateAsync(string id, InventoryItem item);
    Task<bool> RestockAsync(string id, int quantity);
    Task<bool> DeleteAsync(string id);
}
