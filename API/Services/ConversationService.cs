using Microsoft.EntityFrameworkCore;
using AskMeAI.API.Entities;
using AskMeAI.API.DbContexts;
using AskMeAI.API.Models;

namespace AskMeAI.API.Services;
public class ConversationService : IConversationService
{
    private readonly AskMeAiDbContext _context;

    public ConversationService(AskMeAiDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Conversation>> GetConversationsByUserIdAsync(Guid userId)
    {
        return await _context.Conversations
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.Messages.Any()
                ? c.Messages.Max(m => m.SentAt)  
                : c.StartedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Conversation>> GetArchivedConversationsByUserIdAsync(Guid userId)
    {
        return await _context.Conversations.IgnoreQueryFilters()
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .OrderByDescending(c => c.Messages.Any()
                ? c.Messages.Max(m => m.SentAt)
                : c.StartedAt)
            .ToListAsync();
    }

    public async Task<Conversation> GetConversationByIdAsync(Guid conversationId)
    {
        return await _context.Conversations
            .Include(c => c.Messages.OrderBy(m => m.SentAt))
            .FirstOrDefaultAsync(c => c.ConversationId == conversationId);
    }

    public async Task<Conversation> GetConversationByIdForUpdateAsync(Guid conversationId)
    {
        return await _context.Conversations.IgnoreQueryFilters()
            .Where(c =>  !c.IsDeleted && c.ConversationId == conversationId)
            .FirstOrDefaultAsync();
    }

    public async Task<Conversation> AddConversationAsync(Conversation conversation)
    {
        await _context.Conversations.AddAsync(conversation);
        await _context.SaveChangesAsync();
        return conversation;  
    }

    public async Task<Conversation> UpdateConversationAsync(Guid conversationId, ConversationForUpdateDto conversationForUpdateDto)
    {
        var trackedConversation = await _context.Conversations.FindAsync(conversationId);
        if (trackedConversation == null) 
            return null;

        if (conversationForUpdateDto.Title != null)
            trackedConversation.Title = conversationForUpdateDto.Title;

        if (conversationForUpdateDto.IsArchived.HasValue)
            trackedConversation.IsArchived = conversationForUpdateDto.IsArchived.Value;

        await _context.SaveChangesAsync();
        return trackedConversation;
    }

    public async Task SoftDeleteConversationAsync(Conversation conversation)
    {
        if (conversation != null && !conversation.IsDeleted)
        {
            conversation.IsDeleted = true;
            conversation.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

}
