namespace AskMeAI.API.Models;
public class MessageDto
{
    public Guid MessageId { get; set; }
    public Guid ConversationId { get; set; }
    public string SenderType { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
}

