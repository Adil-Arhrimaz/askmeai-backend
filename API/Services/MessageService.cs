using Microsoft.EntityFrameworkCore;
using AskMeAI.API.Entities;
using AskMeAI.API.DbContexts;

namespace AskMeAI.API.Services;
public class MessageService : IMessageService
{
    private readonly AskMeAiDbContext _context;

    public MessageService(AskMeAiDbContext context)
    {
        _context = context;
    }

    public async Task<Message> AddMessageAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();

        return message;
    }

    public async Task<IEnumerable<Message>> GetMessagesForConversationAsync(Guid conversationId)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }
}