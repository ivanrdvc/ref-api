using Microsoft.SemanticKernel.ChatCompletion;

using RefApi.Features.Chat.Models;

namespace RefApi.Features.Chat;

public static class ChatMessageExtensions
{
    public static ChatHistory ToChatHistory(this IEnumerable<ChatMessage> messages)
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