using RefApi.Features.Chat;
using RefApi.Features.Chat.Models;

namespace RefApi.Features.Conversations;

public static class ConversationExtensions
{
    public static ConversationMessageDto ToConversationMessage(
        this (Message? User, Message? Assistant) messagePair, 
        Guid sessionId)
    {
        return new ConversationMessageDto
        {
            User = messagePair.User?.Content ?? string.Empty,
            Response = new ChatResponse
            {
                Message = new ChatMessage
                {
                    Content = messagePair.Assistant?.Content ?? string.Empty,
                    Role = MessageRole.Assistant
                },
                Delta = new ChatMessage { Content = string.Empty, Role = string.Empty },
                Context = new(),
                SessionState = sessionId
            }
        };
    }

    public static List<ConversationMessageDto> ToConversationMessages(
        this IEnumerable<Message> messages,
        Guid sessionId)
    {
        return messages
            .Chunk(2)
            .Select(pair => (
                    User: pair.FirstOrDefault(m => m.Role == MessageRole.User),
                    Assistant: pair.FirstOrDefault(m => m.Role == MessageRole.Assistant))
                .ToConversationMessage(sessionId))
            .ToList();
    }
}