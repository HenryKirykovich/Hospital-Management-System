using HospitalManagement.Server.Hubs;
using HospitalManagement.Server.Services;
using HospitalManagement.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HospitalManagement.Server.Controllers;

/// <summary>
/// REST API for medical inventory management.
/// Any stock change that results in a low-stock condition broadcasts
/// a SignalR "LowStockAlert" to all staff (Admin, Doctor, Nurse groups).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly IHubContext<HospitalHub> _hub;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(
        IInventoryService inventoryService,
        IHubContext<HospitalHub> hub,
        ILogger<InventoryController> logger)
    {
        _inventoryService = inventoryService;
        _hub = hub;
        _logger = logger;
    }

    /// <summary>GET /api/inventory — all items sorted by category then name</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _inventoryService.GetAllAsync();
        return Ok(items);
    }

    /// <summary>GET /api/inventory/low-stock — items at or below their threshold</summary>
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        var items = await _inventoryService.GetLowStockAsync();
        return Ok(items);
    }

    /// <summary>GET /api/inventory/search?q=... — search by name, category, supplier</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var items = await _inventoryService.SearchAsync(q);
        return Ok(items);
    }

    /// <summary>GET /api/inventory/{id}</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var item = await _inventoryService.GetByIdAsync(id);
        if (item is null) return NotFound(new { message = $"Item '{id}' not found." });
        return Ok(item);
    }

    /// <summary>POST /api/inventory — creates a new inventory item</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> Create([FromBody] InventoryItem item)
    {
        var created = await _inventoryService.CreateAsync(item);
        await _hub.Clients.All.SendAsync("InventoryUpdated", created);

        // Immediately alert if created with low stock
        if (created.IsLowStock)
            await BroadcastLowStockAlert(created);

        _logger.LogInformation("Inventory item created: {Name} (qty: {Qty})", item.Name, item.Quantity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>PUT /api/inventory/{id} — updates item details</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> Update(string id, [FromBody] InventoryItem item)
    {
        var updated = await _inventoryService.UpdateAsync(id, item);
        if (!updated) return NotFound(new { message = $"Item '{id}' not found." });

        item.Id = id;
        await _hub.Clients.All.SendAsync("InventoryUpdated", item);

        if (item.IsLowStock)
            await BroadcastLowStockAlert(item);

        return NoContent();
    }

    /// <summary>
    /// PATCH /api/inventory/{id}/restock — adds quantity to current stock.
    /// Broadcasts InventoryUpdated to all clients and clears any low-stock state.
    /// </summary>
    [HttpPatch("{id}/restock")]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> Restock(string id, [FromBody] RestockRequest request)
    {
        if (request.Quantity <= 0)
            return BadRequest(new { message = "Restock quantity must be greater than zero." });

        var success = await _inventoryService.RestockAsync(id, request.Quantity);
        if (!success) return NotFound(new { message = $"Item '{id}' not found." });

        // Fetch updated item to broadcast accurate stock level
        var updated = await _inventoryService.GetByIdAsync(id);
        if (updated is not null)
        {
            await _hub.Clients.All.SendAsync("InventoryUpdated", updated);
            _logger.LogInformation("Restocked '{Name}' by {Qty}. New total: {Total}",
                updated.Name, request.Quantity, updated.Quantity);
        }

        return NoContent();
    }

    /// <summary>DELETE /api/inventory/{id} — removes an item (Admin only)</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _inventoryService.DeleteAsync(id);
        if (!deleted) return NotFound(new { message = $"Item '{id}' not found." });

        await _hub.Clients.All.SendAsync("InventoryDeleted", id);
        return NoContent();
    }

    /// <summary>
    /// Sends a LowStockAlert to Admin, Doctor, and Nurse groups via SignalR.
    /// </summary>
    private async Task BroadcastLowStockAlert(InventoryItem item)
    {
        var alert = new
        {
            itemId = item.Id,
            name = item.Name,
            quantity = item.Quantity,
            threshold = item.LowStockThreshold,
            unit = item.Unit,
            message = $"LOW STOCK: {item.Name} — only {item.Quantity} {item.Unit} remaining (threshold: {item.LowStockThreshold})"
        };

        // Target only staff roles — patients do not see stock alerts
        await _hub.Clients.Groups("Admin", "Doctor", "Nurse")
            .SendAsync("LowStockAlert", alert);

        _logger.LogWarning("Low stock alert: {Name} — {Qty} {Unit} remaining", item.Name, item.Quantity, item.Unit);
    }
}

/// <summary>Payload for PATCH restock endpoint</summary>
public record RestockRequest(int Quantity);
