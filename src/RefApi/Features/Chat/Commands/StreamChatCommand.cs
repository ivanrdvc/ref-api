using System.Runtime.CompilerServices;

using MediatR;

using Microsoft.SemanticKernel.ChatCompletion;

using RefApi.Features.Chat.Mapping;
using RefApi.Features.Chat.Models;
using RefApi.Services;

namespace RefApi.Features.Chat.Commands;

public sealed record StreamChatCommand(
    IReadOnlyList<ChatMessage> Messages,
    ChatRequestContext Context,
    string? SessionState) : IStreamRequest<ChatResponse>;

public class StreamChatCommandHandler(IChatCompletionService chat, IChatOptionsService options)
    : IStreamRequestHandler<StreamChatCommand, ChatResponse>
{
    public async IAsyncEnumerable<ChatResponse> Handle(
        StreamChatCommand request,
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
    {
        var sessionState = request.SessionState ?? Guid.NewGuid().ToString();
        var execSettings = options.GetExecutionSettings(request.Context.Overrides);
        var chatHistory = ChatHistoryMapper.CreateFromMessages(request.Messages);
        chatHistory.AddSystemMessage(options.GetSystemPrompt(request.Context.Overrides));

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