using CareerConnect.Models;
using CareerConnect.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly MessageService _messageService;

    public MessageController(MessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] Message message)
    {
        var result = await _messageService.SendMessageAsync(message);
        return Ok(result);
    }

    [HttpGet("conversation/{userId1}/{userId2}")]
    public async Task<IActionResult> GetConversation(string userId1, string userId2)
    {
        var messages = await _messageService.GetConversationAsync(userId1, userId2);
        return Ok(messages);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserMessages(string userId)
    {
        var messages = await _messageService.GetUserMessagesAsync(userId);
        return Ok(messages);
    }
}