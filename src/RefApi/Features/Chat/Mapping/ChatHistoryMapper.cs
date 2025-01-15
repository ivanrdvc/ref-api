using Microsoft.SemanticKernel.ChatCompletion;

using RefApi.Features.Chat.Models;

namespace RefApi.Features.Chat.Mapping;

public static class ChatHistoryMapper
{
    public static ChatHistory CreateFromMessages(IReadOnlyList<ChatMessage> messages)
    {
        var chatHistory = new ChatHistory();

        foreach (var message in messages)
        {
            var authorRole = message.Role switch
            {
                MessageRole.System => AuthorRole.System,
                MessageRole.User => AuthorRole.User,
                MessageRole.Assistant => AuthorRole.Assistant,
                MessageRole.Tool => AuthorRole.Tool,
                _ => throw new ArgumentException($"Requested value '{message.Role}' was not found.")
            };

            chatHistory.AddMessage(authorRole, message.Content);
        }

        return chatHistory;
    }
}