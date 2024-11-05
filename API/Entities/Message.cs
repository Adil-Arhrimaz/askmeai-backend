using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AskMeAI.API.Models;

namespace AskMeAI.API.Entities;
[Table("messages")]
public class Message
{
    [
        Key, 
        Required, 
        Column("message_id")
    ]
    public Guid MessageId { get; set; }

    [
        Required, 
        Column("conversation_id"), 
        ForeignKey("Conversation")
    ]
    public Guid ConversationId { get; set; }

    public Conversation Conversation { get; set; }

    [
        Required, 
        Column("sender_type")
    ]
    public SenderType SenderType { get; set; }

    [
        Required, 
        Column("content")
    ]
    public string Content { get; set; }

    [
        Required, 
        Column("sent_at")
    ]
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}