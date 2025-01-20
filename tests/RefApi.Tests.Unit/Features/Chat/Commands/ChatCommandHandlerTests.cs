using FluentAssertions;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using NSubstitute;

using RefApi.Features.Chat;
using RefApi.Features.Chat.Commands;
using RefApi.Features.Chat.Models;
using RefApi.Options;
using RefApi.Services;

namespace RefApi.Tests.Unit.Features.Chat.Commands;

public class ChatCommandHandlerTests
{
    private readonly ChatCommandHandler _handler;
    private readonly IChatCompletionService _chat;

    public ChatCommandHandlerTests()
    {
        _chat = Substitute.For<IChatCompletionService>();
        var providerFactory = Substitute.For<IAIProviderFactory>();

        var promptOptions = new PromptOptions { Prompt = "prompt" };
        var options = Substitute.For<IOptions<PromptOptions>>();
        options.Value.Returns(promptOptions);
        
        providerFactory
            .CreateExecutionSettings(Arg.Any<ChatRequestOverrides>())
            .Returns(new OpenAIPromptExecutionSettings());

        _handler = new ChatCommandHandler(_chat, options, providerFactory);
    }

    [Fact]
    public async Task ShouldProcessChatMessageAndReturnResponse()
    {
        // Arrange
        var command = CreateCommand();
        SetupChatResponse("Test response");

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Message!.Content.Should().Be("Test response");
        response.Message.Role.Should().Be(MessageRole.Assistant);
        response.Context!.FollowupQuestions.Should().BeNull();
    }

    [Fact]
    public async Task ShouldGenerateSessionStateWhenNull()
    {
        // Arrange
        var command = CreateCommand();
        SetupChatResponse("Test response");

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.SessionState.Should().NotBeNull();
        Guid.TryParse(response.SessionState!.ToString(), out _).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReuseProvidedSessionState()
    {
        // Arrange
        var existingSession = "existing-session";
        var command = CreateCommand(existingSession);
        SetupChatResponse("Test response");

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.SessionState.Should().Be(existingSession);
    }

    private static ChatCommand CreateCommand(string? sessionState = null) => new(
        Messages: [new ChatMessage { Role = MessageRole.Assistant, Content = "Test" }],
        Context: new() { Overrides = new() },
        SessionState: sessionState);

    private void SetupChatResponse(string content)
    {
        _chat.GetChatMessageContentsAsync(
                Arg.Any<ChatHistory>(),
                executionSettings: Arg.Any<OpenAIPromptExecutionSettings>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ChatMessageContent>>(
                new List<ChatMessageContent> { new() { Content = content, Role = AuthorRole.Assistant } }));
    }
}