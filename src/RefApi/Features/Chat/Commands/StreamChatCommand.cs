using System.Runtime.CompilerServices;

using MediatR;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;

using RefApi.Configuration;
using RefApi.Features.Chat.Models;

namespace RefApi.Features.Chat.Commands;

public sealed record StreamChatCommand(
    IReadOnlyList<ChatMessage> Messages,
    ChatRequestContext Context,
    string? SessionState) : IStreamRequest<ChatResponse>;

public class StreamChatCommandHandler(
    IChatCompletionService chat,
    IAIProviderConfiguration provider) : IStreamRequestHandler<StreamChatCommand, ChatResponse>
{
    public async IAsyncEnumerable<ChatResponse> Handle(
        StreamChatCommand request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var sessionState = request.SessionState ?? Guid.NewGuid().ToString();
        var execSettings = provider.GetExecutionSettings(request.Context.Overrides);
        var chatHistory = request.Messages.ToChatHistory();

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