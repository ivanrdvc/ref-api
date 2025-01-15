using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using RefApi.Common.Mapping;
using RefApi.Features.Chat;
using RefApi.Features.Conversations;
using RefApi.Features.Conversations.Commands;

namespace RefApi.Tests.Unit.Features.Conversations.Commands;

public class SaveConversationCommandHandlerTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture;
    private readonly SaveConversationCommandHandler _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly string _userId = "test-user-id";

    public SaveConversationCommandHandlerTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
        var userContext = new TestUserContext(_userId);
        _handler = new SaveConversationCommandHandler(_databaseFixture.DbContext, userContext);
    }

    [Fact]
    public async Task ShouldCreateNewConversation()
    {
        // Arrange
        var id = Guid.NewGuid();
        var messages = Builder.CreateUserAssistantPairs(("Test message", "Test response"));
        var command = new SaveConversationCommand(id, messages);

        // Act
        await _handler.Handle(command, _cancellationToken);

        // Assert
        var savedConversation = await _databaseFixture.DbContext.Conversations
            .Include(c => c.Messages)
            .SingleAsync(c => c.Id == id && c.UserId == _userId, _cancellationToken);

        savedConversation.Should().NotBeNull();
        savedConversation.Title.Should().Be("Test message");
        savedConversation.Messages.Should().HaveCount(2);
        savedConversation.UserId.Should().Be(_userId);
        savedConversation.Messages.Should().Contain(m =>
            m.Role == MessageRole.User && m.Content == "Test message");
        savedConversation.Messages.Should().Contain(m =>
            m.Role == MessageRole.Assistant && m.Content == "Test response");
    }

    [Fact]
    public async Task ShouldUpdateExistingConversation()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingConversation = new Conversation
        {
            Id = id,
            UserId = _userId,
            Title = "Existing conversation",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Messages = Builder.CreateDomainMessagePair("Initial message", "Initial response")
        };

        await _databaseFixture.DbContext.Conversations.AddAsync(existingConversation, _cancellationToken);
        await _databaseFixture.DbContext.SaveChangesAsync(_cancellationToken);
        var messages = MessageMapper.ToConversationMessages(
            Builder.CreateDomainMessagePair("Initial message", "Initial response")
                .Concat(Builder.CreateDomainMessagePair("Follow up message", "Follow up response")),
            id);

        var command = new SaveConversationCommand(id, messages);

        // Act
        await _handler.Handle(command, _cancellationToken);

        // Assert
        var updatedConversation = await _databaseFixture.DbContext.Conversations
            .Include(c => c.Messages)
            .SingleAsync(c => c.Id == id, _cancellationToken);

        updatedConversation.Messages.Should().HaveCount(4);
        updatedConversation.Messages.Should().Contain(m =>
            m.Role == MessageRole.User && m.Content == "Follow up message");
        updatedConversation.Messages.Should().Contain(m =>
            m.Role == MessageRole.Assistant && m.Content == "Follow up response");
    }

    [Fact]
    public async Task ShouldReturnFalseWhenUserIdIsNull()
    {
        // Arrange
        var unauthenticatedUserContext = new TestUserContext(null!);
        var handler = new SaveConversationCommandHandler(_databaseFixture.DbContext, unauthenticatedUserContext);
        var messages = Builder.CreateUserAssistantPairs(("Test message", "Test response"));
        var command = new SaveConversationCommand(Guid.NewGuid(), messages);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }
}