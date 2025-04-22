using FluentAssertions;

using RefApi.Features.Chat;
using RefApi.Features.Conversations;
using RefApi.Features.Conversations.Queries;

namespace RefApi.Tests.Unit.Features.Conversations.Queries;

public class GetConversationQueryHandlerTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture;
    private readonly GetConversationQueryHandler _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly string _userId = "test-user-id";

    public GetConversationQueryHandlerTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
        var userContext = new TestUserContext(_userId);
        _handler = new GetConversationQueryHandler(_databaseFixture.DbContext, userContext);
    }

    [Fact]
    public async Task ShouldReturnConversationMessages()
    {
        // Arrange
        var id = Guid.NewGuid();
        var conversation = new Conversation
        {
            Id = id,
            UserId = _userId,
            Title = "Test Conversation",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Messages = Builder.CreateDomainMessagePair("Test message", "Test response")
        };

        await _databaseFixture.DbContext.Conversations.AddAsync(conversation, _cancellationToken);
        await _databaseFixture.DbContext.SaveChangesAsync(_cancellationToken);

        var query = new GetConversationQuery(id);

        // Act
        var result = await _handler.HandleAsync(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Should().HaveCount(1);

        var message = result!.First();
        message.User.Should().Be("Test message");
        message.Response.Message!.Content.Should().Be("Test response");
        message.Response.Message.Role.Should().Be(MessageRole.Assistant.ToString().ToLower());
        message.Response.SessionState.Should().Be(id);
    }

    [Fact]
    public async Task ShouldReturnNullWhenConversationBelongsToAnotherUser()
    {
        // Arrange
        var id = Guid.NewGuid();
        var conversation = new Conversation
        {
            Id = id,
            UserId = "different-user-id",
            Title = "Test Conversation",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Messages = Builder.CreateDomainMessagePair("Test message", "Test response")
        };

        await _databaseFixture.DbContext.Conversations.AddAsync(conversation, _cancellationToken);
        await _databaseFixture.DbContext.SaveChangesAsync(_cancellationToken);

        var query = new GetConversationQuery(id);

        // Act
        var result = await _handler.HandleAsync(query, _cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnNullWhenUserIsNotAuthenticated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var unauthenticatedUserContext = new TestUserContext(null!);
        var handler = new GetConversationQueryHandler(_databaseFixture.DbContext, unauthenticatedUserContext);
        var conversation = new Conversation
        {
            Id = id,
            UserId = _userId,
            Title = "Test Conversation",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Messages = Builder.CreateDomainMessagePair("Test message", "Test response")
        };

        await _databaseFixture.DbContext.Conversations.AddAsync(conversation, _cancellationToken);
        await _databaseFixture.DbContext.SaveChangesAsync(_cancellationToken);

        var query = new GetConversationQuery(id);

        // Act
        var result = await handler.HandleAsync(query, _cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnCorrectlyMappedMessages()
    {
        // Arrange
        var id = Guid.NewGuid();
        var conversation = new Conversation
        {
            Id = id,
            UserId = _userId,
            Title = "Test Conversation",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Messages = Builder.CreateDomainMessagePair("First message", "First response")
                .Concat(Builder.CreateDomainMessagePair("Second message", "Second response"))
                .ToList()
        };

        await _databaseFixture.DbContext.Conversations.AddAsync(conversation, _cancellationToken);
        await _databaseFixture.DbContext.SaveChangesAsync(_cancellationToken);

        var query = new GetConversationQuery(id);

        // Act
        var result = await _handler.HandleAsync(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Should().HaveCount(2);

        result![0].User.Should().Be("First message");
        result[0].Response.Message!.Content.Should().Be("First response");

        result[1].User.Should().Be("Second message");
        result[1].Response.Message!.Content.Should().Be("Second response");

        // Verify common properties for all messages
        foreach (var message in result)
        {
            message.Response.Message!.Role.Should().Be(MessageRole.Assistant);
            message.Response.SessionState.Should().Be(id);
            message.Response.Delta.Should().NotBeNull();
            message.Response.Delta!.Content.Should().BeEmpty();
            message.Response.Delta.Role.Should().BeEmpty();
        }
    }
}