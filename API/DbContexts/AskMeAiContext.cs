using Microsoft.EntityFrameworkCore;
using AskMeAI.API.Entities;
using AskMeAI.API.Models;

namespace AskMeAI.API.DbContexts;

public class AskMeAiDbContext : DbContext
{
    public AskMeAiDbContext(DbContextOptions<AskMeAiDbContext> options) : base(options) { }
    public virtual DbSet<Conversation> Conversations { get; set; }
    public virtual DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId);

        modelBuilder.Entity<Conversation>()
            .HasQueryFilter(x => !x.IsDeleted && !x.IsArchived);

        modelBuilder.Entity<Conversation>()
            .HasIndex(x => x.IsDeleted)
            .HasFilter("\"is_deleted\" = false");

        modelBuilder.Entity<Conversation>()
            .HasIndex(c => c.Title)
            .IsUnique();

        modelBuilder.Entity<Message>()
        .HasQueryFilter(m => !m.Conversation.IsDeleted);

        modelBuilder.HasPostgresEnum<SenderType>("public", "sender_type");

        modelBuilder.HasPostgresExtension("uuid-ossp");

        base.OnModelCreating(modelBuilder);

    }
}
