using MediatR;

using Microsoft.SemanticKernel.ChatCompletion;

using RefApi.Features.Chat.Mapping;
using RefApi.Features.Chat.Models;
using RefApi.Services;

namespace RefApi.Features.Chat.Commands;

public record ChatCommand(
    IReadOnlyList<ChatMessage> Messages,
    ChatRequestContext Context,
    string? SessionState) : IRequest<ChatResponse>;

public class ChatCommandHandler(IChatCompletionService chat, IChatOptionsService options)
    : IRequestHandler<ChatCommand, ChatResponse>
{
    public async Task<ChatResponse> Handle(
        ChatCommand request,
        CancellationToken cancellationToken)
    {
        var sessionState = request.SessionState ?? Guid.NewGuid().ToString();
        var execSettings = options.GetExecutionSettings(request.Context.Overrides);
        var chatHistory = ChatHistoryMapper.CreateFromMessages(request.Messages);
        chatHistory.AddSystemMessage(options.GetSystemPrompt(request.Context.Overrides));

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