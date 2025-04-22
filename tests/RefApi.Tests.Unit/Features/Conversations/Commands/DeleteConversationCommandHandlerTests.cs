using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using RefApi.Features.Conversations;
using RefApi.Features.Conversations.Commands;

namespace RefApi.Tests.Unit.Features.Conversations.Commands;

public class DeleteConversationCommandHandlerTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture;
    private readonly DeleteConversationCommandHandler _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly string _userId = "test-user-id";

    public DeleteConversationCommandHandlerTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
        var userContext = new TestUserContext(_userId);
        _handler = new DeleteConversationCommandHandler(_databaseFixture.DbContext, userContext);
    }

    [Fact]
    public async Task ShouldDeleteExistingConversation()
    {
        // Arrange
        var id = Guid.NewGuid();
        var conversation = new Conversation
        {
            Id = id,
            UserId = _userId,
            Title = "Test conversation",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Messages = Builder.CreateDomainMessagePair("Test message", "Test response")
        };

        await _databaseFixture.DbContext.Conversations.AddAsync(conversation, _cancellationToken);
        await _databaseFixture.DbContext.SaveChangesAsync(_cancellationToken);

        var command = new DeleteConversationCommand(id);

        // Act
        var result = await _handler.HandleAsync(command, _cancellationToken);

        // Assert
        result.Should().BeTrue();
        var deletedConversation = await _databaseFixture.DbContext.Conversations
            .FirstOrDefaultAsync(c => c.Id == id, _cancellationToken);
        deletedConversation.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnFalseWhenConversationDoesNotExist()
    {
        // Arrange
        var command = new DeleteConversationCommand(Guid.NewGuid());

        // Act
        var result = await _handler.HandleAsync(command, _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldReturnFalseWhenConversationBelongsToAnotherUser()
    {
        // Arrange
        var id = Guid.NewGuid();
        var conversation = new Conversation
        {
            Id = id,
            UserId = "different-user-id",
            Title = "Test conversation",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Messages = Builder.CreateDomainMessagePair("Test message", "Test response")
        };

        await _databaseFixture.DbContext.Conversations.AddAsync(conversation, _cancellationToken);
        await _databaseFixture.DbContext.SaveChangesAsync(_cancellationToken);

        var command = new DeleteConversationCommand(id);

        // Act
        var result = await _handler.HandleAsync(command, _cancellationToken);

        // Assert
        result.Should().BeFalse();
        var existingConversation = await _databaseFixture.DbContext.Conversations
            .FirstOrDefaultAsync(c => c.Id == id, _cancellationToken);
        existingConversation.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldReturnFalseWhenUserIdIsNull()
    {
        // Arrange
        var unauthenticatedUserContext = new TestUserContext(null!);
        var handler = new DeleteConversationCommandHandler(_databaseFixture.DbContext, unauthenticatedUserContext);
        var command = new DeleteConversationCommand(Guid.NewGuid());

        // Act
        var result = await handler.HandleAsync(command, _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }
}