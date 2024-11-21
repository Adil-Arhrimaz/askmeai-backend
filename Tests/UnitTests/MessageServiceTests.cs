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
using Xunit;
using FluentAssertions;

public class MessageServiceTests
{
    private readonly Mock<AskMeAiDbContext> _contextMock;
    private readonly MessageService _messageService;

    public MessageServiceTests()
    {
        var options = new DbContextOptionsBuilder<AskMeAiDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _contextMock = new Mock<AskMeAiDbContext>(options);
        _messageService = new MessageService(_contextMock.Object);
    }

    [Fact]
    public async Task AddMessageAsync_ShouldAddAndReturnMessage()
    {
        // Arrange
        var message = new Message
        {
            MessageId = Guid.NewGuid(),
            ConversationId = Guid.NewGuid(),
            SentAt = DateTime.UtcNow,
            Content = "Test Message"
        };

        var messages = new List<Message>();

        _contextMock.Setup(c => c.Messages).ReturnsDbSet(messages);

        // Act
        var result = await _messageService.AddMessageAsync(message);

        // Assert
        _contextMock.Verify(m => m.Messages.AddAsync(message, default), Times.Once);
        _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        result.Should().Be(message);
    }

    [Fact]
    public async Task GetMessagesForConversationAsync_ShouldReturnMessagesOrderedBySentAt()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var messages = new List<Message>
        {
            new Message { MessageId = Guid.NewGuid(), ConversationId = conversationId, SentAt = DateTime.UtcNow.AddMinutes(-10), Content = "Message 1" },
            new Message { MessageId = Guid.NewGuid(), ConversationId = conversationId, SentAt = DateTime.UtcNow.AddMinutes(-5), Content = "Message 2" }
        };

        _contextMock.Setup(c => c.Messages).ReturnsDbSet(messages);

        // Act
        var result = await _messageService.GetMessagesForConversationAsync(conversationId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeInAscendingOrder(m => m.SentAt);
        result.All(m => m.ConversationId == conversationId).Should().BeTrue();
    }

    [Fact]
    public async Task GetMessagesForConversationAsync_ShouldReturnEmptyListIfNoMessagesExist()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var messages = new List<Message>();

        _contextMock.Setup(c => c.Messages).ReturnsDbSet(messages);

        // Act
        var result = await _messageService.GetMessagesForConversationAsync(conversationId);

        // Assert
        result.Should().BeEmpty();
    }
}
