using FluentAssertions;

using RefApi.Features.Conversations;
using RefApi.Features.Conversations.Queries;

namespace RefApi.Tests.Unit.Features.Conversations.Queries;

public class GetConversationsQueryHandlerTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture;
    private readonly GetConversationsQueryHandler _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly string _userId = "test-user-id";

    public GetConversationsQueryHandlerTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
        var userContext = new TestUserContext(_userId);
        _handler = new GetConversationsQueryHandler(_databaseFixture.DbContext, userContext);
    }

    [Fact]
    public async Task ShouldReturnEmptyResponseWhenNoConversations()
    {
        // Arrange
        var query = new GetConversationsQuery(10);

        // Act
        var result = await _handler.HandleAsync(query, _cancellationToken);

        // Assert
        result.Items.Should().BeEmpty();
        result.ContinuationToken.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnEmptyResponseWhenUserIsNotAuthenticated()
    {
        // Arrange
        var unauthenticatedUserContext = new TestUserContext(null!);
        var handler = new GetConversationsQueryHandler(_databaseFixture.DbContext, unauthenticatedUserContext);
        var conversations = new List<Conversation>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                Title = "Test Conversation",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Messages = Builder.CreateDomainMessagePair("Test message", "Test response")
            }
        };

        await _databaseFixture.DbContext.Conversations.AddRangeAsync(conversations, _cancellationToken);
        await _databaseFixture.DbContext.SaveChangesAsync(_cancellationToken);

        var query = new GetConversationsQuery(10);

        // Act
        var result = await handler.HandleAsync(query, _cancellationToken);

        // Assert
        result.Items.Should().BeEmpty();
        result.ContinuationToken.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnOnlyUserConversations()
    {
        // Arrange
        var conversations = new List<Conversation>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                Title = "User Conversation",
                Timestamp = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeMilliseconds(),
                Messages = Builder.CreateDomainMessagePair("User message", "User response")
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = "different-user-id",
                Title = "Other User Conversation",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Messages = Builder.CreateDomainMessagePair("Other message", "Other response")
            }
        };

        await _databaseFixture.DbContext.Conversations.AddRangeAsync(conversations, _cancellationToken);
        await _databaseFixture.DbContext.SaveChangesAsync(_cancellationToken);

        var query = new GetConversationsQuery(10);

        // Act
        var result = await _handler.HandleAsync(query, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(1);
        result.ContinuationToken.Should().BeNull();

        var item = result.Items.Single();
        item.Title.Should().Be("User Conversation");
    }

    [Fact]
    public async Task ShouldReturnConversationsOrderedByTimestamp()
    {
        // Arrange
        var conversations = new List<Conversation>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                Title = "First Conversation",
                Timestamp = DateTimeOffset.UtcNow.AddHours(-2).ToUnixTimeMilliseconds(),
                Messages = Builder.CreateDomainMessagePair("First message", "First response")
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                Title = "Second Conversation",
                Timestamp = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeMilliseconds(),
                Messages = Builder.CreateDomainMessagePair("Second message", "Second response")
            }
        };

        await _databaseFixture.DbContext.Conversations.AddRangeAsync(conversations, _cancellationToken);
        await _databaseFixture.DbContext.SaveChangesAsync(_cancellationToken);

        var query = new GetConversationsQuery(10);

        // Act
        var result = await _handler.HandleAsync(query, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(2);
        result.ContinuationToken.Should().BeNull();

        var items = result.Items.ToList();
        items[0].Title.Should().Be("Second Conversation");
        items[1].Title.Should().Be("First Conversation");
    }

    [Fact]
    public async Task ShouldHandlePagination()
    {
        // Arrange
        var conversations = Enumerable.Range(0, 5).Select(i => new Conversation
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            Title = $"Conversation {i}",
            Timestamp = DateTimeOffset.UtcNow.AddHours(-i).ToUnixTimeMilliseconds(),
            Messages = Builder.CreateDomainMessagePair($"Message {i}", $"Response {i}")
        }).ToList();

        await _databaseFixture.DbContext.Conversations.AddRangeAsync(conversations, _cancellationToken);
        await _databaseFixture.DbContext.SaveChangesAsync(_cancellationToken);

        // First page
        var firstQuery = new GetConversationsQuery(2);
        var firstResult = await _handler.HandleAsync(firstQuery, _cancellationToken);

        firstResult.Items.Should().HaveCount(2);
        firstResult.ContinuationToken.Should().NotBeNull();

        var firstPage = firstResult.Items.ToList();
        firstPage[0].Title.Should().Be("Conversation 0");
        firstPage[1].Title.Should().Be("Conversation 1");

        // Second page
        var secondQuery = new GetConversationsQuery(2, firstResult.ContinuationToken);
        var secondResult = await _handler.HandleAsync(secondQuery, _cancellationToken);

        secondResult.Items.Should().HaveCount(2);
        secondResult.ContinuationToken.Should().NotBeNull();

        var secondPage = secondResult.Items.ToList();
        secondPage[0].Title.Should().Be("Conversation 2");
        secondPage[1].Title.Should().Be("Conversation 3");

        // Last page
        var lastQuery = new GetConversationsQuery(2, secondResult.ContinuationToken);
        var lastResult = await _handler.HandleAsync(lastQuery, _cancellationToken);

        lastResult.Items.Should().HaveCount(1);
        lastResult.ContinuationToken.Should().BeNull();

        var lastPage = lastResult.Items.ToList();
        lastPage[0].Title.Should().Be("Conversation 4");
    }

    [Fact]
    public async Task ShouldHandleInvalidContinuationToken()
    {
        // Arrange
        var query = new GetConversationsQuery(10, "invalid");

        // Act/Assert
        await Assert.ThrowsAsync<FormatException>(() => _handler.HandleAsync(query, _cancellationToken));
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _databaseFixture.ResetDatabase();
}