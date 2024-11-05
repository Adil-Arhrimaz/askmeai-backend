using AskMeAI.API.Entities;

namespace AskMeAI.API.Services;
public interface IMessageService
{
    Task<Message> AddMessageAsync(Message message);
    Task<IEnumerable<Message>> GetMessagesForConversationAsync(Guid conversationId);
}