using HospitalManagement.Server.Services;
using HospitalManagement.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Server.Controllers;

/// <summary>
/// REST API for retrieving chat history.
/// Sending messages is done through the SignalR hub (SendMessage method) for real-time delivery.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>GET /api/chat/{channel}?limit=50 — load recent messages for a channel</summary>
    [HttpGet("{channel}")]
    public async Task<IActionResult> GetHistory(string channel, [FromQuery] int limit = 50)
    {
        if (limit < 1 || limit > 200) limit = 50;
        var messages = await _chatService.GetChannelHistoryAsync(channel, limit);
        return Ok(messages);
    }

    /// <summary>POST /api/chat/{channel}/read — mark all messages as read for the calling user</summary>
    [HttpPost("{channel}/read")]
    public async Task<IActionResult> MarkRead(string channel)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        await _chatService.MarkChannelReadAsync(channel, userId);
        return NoContent();
    }
}
