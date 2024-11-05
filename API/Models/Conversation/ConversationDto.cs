namespace AskMeAI.API.Models;
public class ConversationDto
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
}