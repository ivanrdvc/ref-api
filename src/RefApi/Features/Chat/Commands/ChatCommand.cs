using MediatR;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;

using RefApi.Configuration;
using RefApi.Extensions;
using RefApi.Features.Chat.Models;
using RefApi.Options;

namespace RefApi.Features.Chat.Commands;

public record ChatCommand(
    IReadOnlyList<ChatMessage> Messages,
    ChatRequestContext Context,
    string? SessionState) : IRequest<ChatResponse>;

public class ChatCommandHandler(
    IChatCompletionService chat,
    IOptions<PromptOptions> options,
    IAIProviderSettings providerSettings) : IRequestHandler<ChatCommand, ChatResponse>
{
    private readonly PromptOptions _options = options.Value;

    public async Task<ChatResponse> Handle(
        ChatCommand request,
        CancellationToken cancellationToken)
    {
        var sessionState = request.SessionState ?? Guid.NewGuid().ToString();
        var execSettings = providerSettings.CreateExecutionSettings(request.Context.Overrides);
        var chatHistory = request.Messages.ToChatHistory();
        chatHistory.AddSystemMessage(request.Context.Overrides.PromptTemplate ?? _options.Prompt);

        var response = await chat.GetChatMessageContentsAsync(
            chatHistory,
            executionSettings: execSettings,
            cancellationToken: cancellationToken);

        return new ChatResponse
        {
            Message = new ChatMessage
            {
                Content = response[0].Content ?? string.Empty,
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