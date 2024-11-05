using System.ComponentModel.DataAnnotations;

namespace AskMeAI.API.Models;
public class MessageForCreationDto
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; }
}
