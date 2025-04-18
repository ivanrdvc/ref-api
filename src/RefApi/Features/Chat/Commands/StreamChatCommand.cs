using System.Runtime.CompilerServices;

using MediatR;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;

using RefApi.Configuration;
using RefApi.Extensions;
using RefApi.Features.Chat.Models;
using RefApi.Options;

namespace RefApi.Features.Chat.Commands;

public sealed record StreamChatCommand(
    IReadOnlyList<ChatMessage> Messages,
    ChatRequestContext Context,
    string? SessionState) : IStreamRequest<ChatResponse>;

public class StreamChatCommandHandler(
    IChatCompletionService chat,
    IOptions<PromptOptions> options,
    IAIProviderSettings providerSettings) : IStreamRequestHandler<StreamChatCommand, ChatResponse>
{
    private readonly PromptOptions _options = options.Value;

    public async IAsyncEnumerable<ChatResponse> Handle(
        StreamChatCommand request,
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
    {
        var sessionState = request.SessionState ?? Guid.NewGuid().ToString();
        var execSettings = providerSettings.CreateExecutionSettings(request.Context.Overrides);
        var chatHistory = request.Messages.ToChatHistory();
        chatHistory.AddSystemMessage(request.Context.Overrides.PromptTemplate ?? _options.Prompt);

        yield return new ChatResponse
        {
            Message = new ChatMessage { Role = MessageRole.Assistant },
            Delta = new ChatMessage { Role = MessageRole.Assistant },
            Context = new ResponseContext
            {
                DataPoints = [],
                FollowupQuestions = null,
                Thoughts = []
            },
            SessionState = sessionState
        };

        await foreach (var response in chat.GetStreamingChatMessageContentsAsync(
                           chatHistory,
                           executionSettings: execSettings,
                           cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(response.Content))
            {
                yield return new ChatResponse
                {
                    Delta = new ChatMessage { Content = response.Content, Role = MessageRole.Assistant }
                };
            }
        }
    }
}