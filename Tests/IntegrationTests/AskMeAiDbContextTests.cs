using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AskMeAI.API.Entities;
using AskMeAI.API.DbContexts;
using AskMeAI.API.Models;
using FluentAssertions;

public class AskMeAiDbContextTests : IClassFixture<DbContextFixture>
{
    private readonly DbContextFixture _fixture;

    public AskMeAiDbContextTests(DbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Can_Insert_And_Retrieve_Conversation_With_Messages()
    {
        using (var context = _fixture.CreateContext())
        {
            var conversation = new Conversation
            {
                ConversationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Title = "Sample Conversation",
                StartedAt = DateTime.UtcNow,
                IsArchived = false,
                IsDeleted = false
            };

            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                ConversationId = conversation.ConversationId,
                SenderType = SenderType.User,
                Content = "Hello, AI!",
                SentAt = DateTime.UtcNow
            };

            conversation.Messages.Add(message);
            context.Conversations.Add(conversation);
            context.SaveChanges();
        

            var conversationResult = context.Conversations
                .Include(c => c.Messages)
                .FirstOrDefault(c => c.Title == "Sample Conversation");

            conversationResult.Should().NotBeNull();
            conversationResult.Title.Should().Be("Sample Conversation");
            conversationResult.Messages.Single().Content.Should().Be("Hello, AI!");
        }
    }

    [Fact]
    public void Can_Save_And_Retrieve_Enum_SenderType_As_Integer()
    {
        using (var context = _fixture.CreateContext())
        {
            var conversation = new Conversation
            {
                ConversationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Title = "Enum Test Conversation",
                StartedAt = DateTime.UtcNow,
                IsArchived = false,
                IsDeleted = false
            };

            context.Conversations.Add(conversation);
            context.SaveChanges();

            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                ConversationId = conversation.ConversationId,
                SenderType = SenderType.AI,
                Content = "I am AI!",
                SentAt = DateTime.UtcNow
            };

            context.Messages.Add(message);
            context.SaveChanges();
        

            var messageResult = context.Messages.FirstOrDefault();

            messageResult.Should().NotBeNull();
            messageResult.SenderType.Should().Be(SenderType.AI);
        }
    }

    [Fact]
    public void Can_Ensure_Default_Values_For_Conversation()
    {
        using (var context = _fixture.CreateContext())
        {
            var conversation = new Conversation
            {
                ConversationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Title = "Conversation Default Test",
                StartedAt = DateTime.UtcNow
            };

            context.Conversations.Add(conversation);
            context.SaveChanges();

            var result = context.Conversations.Find(conversation.ConversationId);

            result.Should().NotBeNull();
            result.IsArchived.Should().BeFalse();
            result.IsDeleted.Should().BeFalse();
        }
    }

    [Fact]
    public void QueryFilter_Should_Exclude_Deleted_Or_Archived_Conversations()
    {
        using (var context = _fixture.CreateContext())
        {
            var conversation1 = new Conversation
            {
                ConversationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Title = "Active Conversation",
                IsDeleted = false,
                IsArchived = false,
                StartedAt = DateTime.UtcNow
            };

            var conversation2 = new Conversation
            {
                ConversationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Title = "Deleted Conversation",
                IsDeleted = true,
                IsArchived = false,
                StartedAt = DateTime.UtcNow
            };

            var conversation3 = new Conversation
            {
                ConversationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Title = "Archived Conversation",
                IsDeleted = false,
                IsArchived = true,
                StartedAt = DateTime.UtcNow
            };

            context.Conversations.AddRange(conversation1, conversation2, conversation3);
            context.SaveChanges();

            var activeConversations = context.Conversations.ToList();

            activeConversations.Single().IsArchived.Should().BeFalse();
            activeConversations.Single().IsDeleted.Should().BeFalse();
            activeConversations.Single().Title.Should().Be("Active Conversation");
        }
    }

    [Fact]
    public void Cannot_Add_Message_Without_Valid_Conversation()
    {
        using (var context = _fixture.CreateContext())
        {
            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                ConversationId = Guid.NewGuid(),
                SenderType = SenderType.User,
                Content = "Invalid message!",
                SentAt = DateTime.UtcNow
            };

            context.Messages.Add(message);

            FluentActions.Invoking(context.SaveChanges)
                .Should().Throw<DbUpdateException>();
        }
    }

    [Fact]
    public void Should_Use_Postgres_UUID_Extension()
    {
        using (var context = _fixture.CreateContext())
        {
            var conversation = new Conversation
            {
                UserId = Guid.NewGuid(),
                Title = "UUID Test",
                StartedAt = DateTime.UtcNow
            };

            context.Conversations.Add(conversation);
            context.SaveChanges();

            var result = context.Conversations.Find(conversation.ConversationId);

            result.Should().NotBeNull();
            result.ConversationId.GetType().Should().Be(typeof(Guid));
        }
    }

    [Fact]
    public void Cannot_Add_Conversation_Without_Required_Fields()
    {
        using (var context = _fixture.CreateContext())
        {
            var invalidConversation = new Conversation
            {
                ConversationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                // Missing Title (which is required)
                StartedAt = DateTime.UtcNow
            };

            context.Conversations.Add(invalidConversation);

            FluentActions.Invoking(context.SaveChanges)
                .Should().Throw<DbUpdateException>();
        }
    }

    [Fact]
    public void Soft_Deleted_Should_Not_Be_Retrievable()
    {
        using (var context = _fixture.CreateContext())
        {
            var conversation = new Conversation
            {
                ConversationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Title = "Soft Delete Test",
                StartedAt = DateTime.UtcNow,
                IsArchived = false,
                IsDeleted = false
            };

            context.Conversations.Add(conversation);
            context.SaveChanges();

            conversation.IsDeleted = true;
            conversation.DeletedAt = DateTime.UtcNow;
            context.SaveChanges();

            var result = context.Conversations.FirstOrDefault(c => c.ConversationId == conversation.ConversationId);

            result.Should().BeNull();
        }
    }
    [Fact]
    public void Cannot_Add_Duplicate_Conversation_Titles()
    {
        using (var context = _fixture.CreateContext())
        {
            var conversation1 = new Conversation
            {
                ConversationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Title = "Unique Title Test",
                StartedAt = DateTime.UtcNow
            };

            var conversation2 = new Conversation
            {
                ConversationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Title = "Unique Title Test",  // Duplicate title
                StartedAt = DateTime.UtcNow
            };

            context.Conversations.Add(conversation1);
            context.Conversations.Add(conversation2);

            FluentActions.Invoking(context.SaveChanges)
                .Should().Throw<DbUpdateException>();

        }
    }

}
