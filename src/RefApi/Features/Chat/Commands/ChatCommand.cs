using Microsoft.SemanticKernel.ChatCompletion;

using RefApi.Common;
using RefApi.Configuration;
using RefApi.Features.Chat.Models;

namespace RefApi.Features.Chat.Commands;

public record ChatCommand(
    IReadOnlyList<ChatMessage> Messages,
    ChatRequestContext Context,
    string? SessionState);

public class ChatCommandHandler(
    IChatCompletionService chat,
    IAIProviderConfiguration provider) : IRequestHandler<ChatCommand, ChatResponse>
{
    public async Task<ChatResponse> HandleAsync(
        ChatCommand request,
        CancellationToken cancellationToken)
    {
        var sessionState = request.SessionState ?? Guid.NewGuid().ToString();
        var execSettings = provider.GetExecutionSettings(request.Context.Overrides);
        var chatHistory = request.Messages.ToChatHistory();

        var response = await chat.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: execSettings,
            cancellationToken: cancellationToken);

        return new ChatResponse
        {
            Message = new ChatMessage
            {
                Content = response.Content ?? string.Empty,
                Role = MessageRole.Assistant
            },
            Context = new ResponseContext
            {
                FollowupQuestions = request.Context.Overrides.SuggestFollowupQuestions ? [] : null,
                DataPoints = [],
                Thoughts = []
            },
            SessionState = sessionState
        };
    }
}