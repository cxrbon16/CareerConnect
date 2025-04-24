using CareerConnect.Data;
using CareerConnect.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.Services;

public class MessageService
{
    private readonly ApplicationDbContext _context;

    public MessageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Message> SendMessageAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<List<Message>> GetConversationAsync(string userId1, string userId2)
    {
        return await _context.Messages
            .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task<List<Message>> GetUserMessagesAsync(string userId)
    {
        return await _context.Messages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();
    }
}