using FluentAssertions;

using RefApi.Features.Chat;
using RefApi.Features.Chat.Mapping;
using RefApi.Features.Chat.Models;

namespace RefApi.Tests.Unit.Features.Chat.Mapping;

public class ChatHistoryMapperTests
{
    [Fact]
    public void ShouldCreateEmptyChatHistory()
    {
        // Arrange
        var messages = new List<ChatMessage>();

        // Act
        var result = ChatHistoryMapper.CreateFromMessages(messages);

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(MessageRole.System, "system")]
    [InlineData(MessageRole.User, "user")]
    [InlineData(MessageRole.Assistant, "assistant")]
    [InlineData(MessageRole.Tool, "tool")]
    public void ShouldMapRoleCorrectly(string messageRole, string expectedAuthorRole)
    {
        // Arrange
        var messages = new List<ChatMessage> { new() { Role = messageRole.ToString(), Content = "Test content" } };

        // Act
        var result = ChatHistoryMapper.CreateFromMessages(messages);

        // Assert
        result.Should().HaveCount(1);
        result[0].Role.ToString().ToLower().Should().Be(expectedAuthorRole);
        result[0].Content.Should().Be("Test content");
    }

    [Fact]
    public void ShouldPreserveMessageOrder()
    {
        // Arrange
        var messages = new List<ChatMessage>
        {
            new() { Role = "user", Content = "First message" },
            new() { Role = "assistant", Content = "Second message" },
            new() { Role = "user", Content = "Third message" }
        };

        // Act
        var result = ChatHistoryMapper.CreateFromMessages(messages);

        // Assert
        result.Should().HaveCount(3);
        result.Select(m => m.Content)
            .Should().ContainInOrder("First message", "Second message", "Third message");
    }

    [Fact]
    public void ShouldThrowForInvalidRole()
    {
        // Arrange
        var invalidRole = "invalid";
        var messages = new List<ChatMessage> { new() { Role = "invalid", Content = "Test content" } };

        // Act & Assert
        var action = () => ChatHistoryMapper.CreateFromMessages(messages);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage($"Requested value '{invalidRole}' was not found.");
    }
}