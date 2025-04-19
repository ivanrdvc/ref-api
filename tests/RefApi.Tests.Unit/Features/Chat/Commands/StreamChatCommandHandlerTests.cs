using FluentAssertions;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using NSubstitute;

using RefApi.Configuration;
using RefApi.Features.Chat;
using RefApi.Features.Chat.Commands;
using RefApi.Features.Chat.Models;

namespace RefApi.Tests.Unit.Features.Chat.Commands;

public class StreamChatCommandHandlerTests
{
    private StreamChatCommandHandler _handler;
    private readonly IChatCompletionService _chat;
    private readonly IAIProviderConfiguration _providerConfigurationMock;

    public StreamChatCommandHandlerTests()
    {
        _chat = Substitute.For<IChatCompletionService>();
        _providerConfigurationMock = Substitute.For<IAIProviderConfiguration>();

        _providerConfigurationMock
            .GetExecutionSettings(Arg.Any<ChatRequestOverrides>())
            .Returns(new OpenAIPromptExecutionSettings());

        _handler = new StreamChatCommandHandler(_chat, _providerConfigurationMock);
    }

    [Fact]
    public async Task ShouldStreamChatMessage()
    {
        // Arrange
        var command = CreateCommand();
        SetupStreamingResponse(["Hello", "world"]);

        // Act
        var responses = await CollectResponses(_handler.Handle(command, CancellationToken.None));

        // Assert
        responses.Should().HaveCount(3, "initial response + 2 content chunks");
        responses[0].Delta?.Content.Should().BeEmpty();
        responses[1].Delta?.Content.Should().Be("Hello");
        responses[2].Delta?.Content.Should().Be("world");
    }

    [Fact]
    public async Task ShouldGenerateSessionStateWhenNull()
    {
        // Arrange
        var command = CreateCommand(sessionState: null);
        SetupStreamingResponse(["test"]);

        // Act
        var responses = await CollectResponses(_handler.Handle(command, CancellationToken.None));

        // Assert
        var sessionState = responses.First().SessionState;
        sessionState.Should().NotBeNull();
        Guid.TryParse(sessionState!.ToString(), out _).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReuseProvidedSessionState()
    {
        // Arrange
        var existingSession = "existing-session";
        var command = CreateCommand(existingSession);
        SetupStreamingResponse(["test"]);

        // Act
        var responses = await CollectResponses(_handler.Handle(command, CancellationToken.None));

        // Assert
        responses.First().SessionState.Should().Be(existingSession);
    }

    [Fact]
    public async Task ShouldUseSystemPromptFromOptions()
    {
        // Arrange
        var command = CreateCommand();
        ChatHistory? capturedHistory = null;

        _chat.GetStreamingChatMessageContentsAsync(
                Arg.Do<ChatHistory>(h => capturedHistory = h),
                executionSettings: Arg.Any<OpenAIPromptExecutionSettings>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(CreateStreamingResponse(["test"]));

        // Act
        await CollectResponses(_handler.Handle(command, CancellationToken.None));

        // Assert
        capturedHistory.Should().NotBeNull();
        capturedHistory!.Should().Contain(m =>
            m.Role.Label == "system" &&
            m.Content == "prompt");
    }

    [Fact]
    public async Task ShouldUseExecutionSettingsFromOptions()
    {
        // Arrange
        var temperature = 0.7;
        var expectedSettings = new OpenAIPromptExecutionSettings { Temperature = temperature };

        // Setup provider factory to return specific settings
        _providerConfigurationMock
            .GetExecutionSettings(Arg.Any<ChatRequestOverrides>())
            .Returns(expectedSettings);

        _handler = new StreamChatCommandHandler(_chat, _providerConfigurationMock);

        var command = CreateCommand();
        SetupStreamingResponse(["test"]);

        // Act
        await CollectResponses(_handler.Handle(command, default));

        // Assert
        _chat.Received(1).GetStreamingChatMessageContentsAsync(
            Arg.Any<ChatHistory>(),
            executionSettings: Arg.Is<OpenAIPromptExecutionSettings>(s => s.Temperature == temperature),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    private static StreamChatCommand CreateCommand(string? sessionState = null) => new(
        Messages: [new ChatMessage { Role = MessageRole.Assistant, Content = "Test" }],
        Context: new() { Overrides = new() },
        SessionState: sessionState);

    private void SetupStreamingResponse(string[] contents) => _chat
        .GetStreamingChatMessageContentsAsync(
            Arg.Any<ChatHistory>(),
            executionSettings: Arg.Any<OpenAIPromptExecutionSettings>(),
            cancellationToken: Arg.Any<CancellationToken>())
        .Returns(CreateStreamingResponse(contents));

    private static async IAsyncEnumerable<StreamingChatMessageContent> CreateStreamingResponse(string[] contents)
    {
        foreach (var content in contents)
        {
            yield return new StreamingChatMessageContent(
                role: AuthorRole.Assistant,
                content: content,
                innerContent: null,
                choiceIndex: 0,
                modelId: null,
                encoding: null,
                metadata: null);
            await Task.CompletedTask;
        }
    }

    private static async Task<List<ChatResponse>> CollectResponses(IAsyncEnumerable<ChatResponse> stream)
    {
        var responses = new List<ChatResponse>();
        await foreach (var response in stream)
        {
            responses.Add(response);
        }

        return responses;
    }
}