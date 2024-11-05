using AskMeAI.API.Entities;
using AskMeAI.API.Models;

namespace AskMeAI.API.Services;
public interface IConversationService
{
    Task<IEnumerable<Conversation>> GetConversationsByUserIdAsync(Guid userId);
    Task<Conversation> GetConversationByIdAsync(Guid conversationId);
    Task<IEnumerable<Conversation>> GetArchivedConversationsByUserIdAsync(Guid userId);
    Task<Conversation> GetConversationByIdForUpdateAsync(Guid conversationId);
    Task<Conversation> AddConversationAsync(Conversation conversation);
    Task<Conversation> UpdateConversationAsync(Guid conversationId, ConversationForUpdateDto conversationForUpdateDto);
    Task SoftDeleteConversationAsync(Conversation conversation);
}
