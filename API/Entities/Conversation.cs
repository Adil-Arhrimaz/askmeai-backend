using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AskMeAI.API.Entities;

[Table("conversations")]
public class Conversation
{
    [
        Key, 
        Required, 
        Column("conversation_id")
    ]
    public Guid ConversationId { get; set; }

    [
        Required, 
        Column("user_id")
    ]
    public Guid UserId { get; set; }

    [
        Required, 
        Column("title"),
        StringLength(100),
    ]
    public string Title { get; set; }

    [
        Required, 
        Column("started_at")
    ]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    [
        Required,
        Column("is_archived")
    ]
    public bool IsArchived { get; set; } = false;

    [
        Required,
        Column("is_deleted")
    ]
    public bool IsDeleted { get; set; } = false;

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    public List<Message> Messages { get; set; } = new List<Message>();

    public Conversation()
    {
        Messages = new List<Message>();
    }
}
