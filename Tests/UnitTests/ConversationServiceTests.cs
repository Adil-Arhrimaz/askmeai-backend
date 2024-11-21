using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskMeAI.API.Entities;
using AskMeAI.API.Services;
using AskMeAI.API.DbContexts;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using FluentAssertions;
using AskMeAI.API.Models;

public class ConversationServiceTests
{
    private readonly Mock<AskMeAiDbContext> _contextMock;
    private readonly ConversationService _conversationService;

    public ConversationServiceTests()
    {
        // Arrange DbContextOptions and pass to the Mock
        var options = new DbContextOptionsBuilder<AskMeAiDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _contextMock = new Mock<AskMeAiDbContext>(options);
        _conversationService = new ConversationService(_contextMock.Object);
    }

    [Fact]
    public async Task GetConversationsByUserIdAsync_ShouldReturnConversations()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var recentMessageDate = DateTime.UtcNow;
        var olderMessageDate = recentMessageDate.AddDays(-1);
        var conversations = new List<Conversation>
        {
            new Conversation
            {
                ConversationId = Guid.NewGuid(),
                UserId = userId,
                Title = "Conversation 1",
                Messages = new List<Message>
                {
                    new Message { SentAt = recentMessageDate },
                    new Message { SentAt = olderMessageDate }
                }
            },
            new Conversation
            {
                ConversationId = Guid.NewGuid(),
                UserId = userId,
                Title = "Conversation 2",
            }
        };

        // Use Moq.EntityFrameworkCore's ReturnsDbSet to simplify setup
        _contextMock.Setup(c => c.Conversations).ReturnsDbSet(conversations);

        // Act
        var result = await _conversationService.GetConversationsByUserIdAsync(userId);

        // Assert
        result.Should().BeOfType<List<Conversation>>();
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(c => c.UserId == userId).Should().BeTrue();
    }

    [Fact]
    public async Task AddConversationAsync_ShouldAddAndReturnConversation()
    {
        // Arrange
        var conversation = new Conversation { ConversationId = Guid.NewGuid(), UserId = Guid.NewGuid(), Title = "New Conversation" };
        var conversations = new List<Conversation>();

        // Set up the Conversations DbSet to use an empty list initially
        _contextMock.Setup(c => c.Conversations).ReturnsDbSet(conversations);

        // Act
        var result = await _conversationService.AddConversationAsync(conversation);

        // Assert
        _contextMock.Verify(m => m.Conversations.AddAsync(conversation, default), Times.Once);
        _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        result.Should().Be(conversation);
    }

    [Fact]
    public async Task SoftDeleteConversationAsync_ShouldSetIsDeletedAndSave()
    {
        // Arrange
        var conversation = new Conversation { ConversationId = Guid.NewGuid(), IsDeleted = false };
        var conversations = new List<Conversation> { conversation };

        // Use ReturnsDbSet to set up the mock
        _contextMock.Setup(c => c.Conversations).ReturnsDbSet(conversations);

        // Act
        await _conversationService.SoftDeleteConversationAsync(conversation);

        // Assert
        conversation.IsDeleted.Should().BeTrue();
        conversation.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateConversationAsync_ShouldUpdateAndReturnConversation()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var conversation = new Conversation
        {
            ConversationId = conversationId,
            Title = "Initial Title",
            IsArchived = false
        };

        var updateDto = new ConversationForUpdateDto
        {
            Title = "Updated Title",
            IsArchived = true
        };

        var conversations = new List<Conversation> { conversation };

        // Mock DbContext and the Conversations DbSet
        _contextMock.Setup(c => c.Conversations).ReturnsDbSet(conversations);
        _contextMock.Setup(c => c.Conversations.FindAsync(conversationId))
                    .ReturnsAsync(conversation);

        // Act
        var result = await _conversationService.UpdateConversationAsync(conversationId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Updated Title");
        result.IsArchived.Should().BeTrue();
        _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }



}
