using RefApi.Features.Chat;
using RefApi.Features.Chat.Models;
using RefApi.Features.Conversations;

namespace RefApi.Common.Mapping;

public static class MessageMapper
{
    public static ConversationMessage ToConversationMessage(
        Message? userMessage,
        Message? assistantMessage,
        Guid sessionId)
    {
        return new ConversationMessage
        {
            User = userMessage?.Content ?? string.Empty,
            Response = new ChatResponse
            {
                Message = new ChatMessage
                {
                    Content = assistantMessage?.Content ?? string.Empty, Role = MessageRole.Assistant
                },
                Delta = new ChatMessage { Content = string.Empty, Role = string.Empty },
                // TODO: Add ResponseContext after implementing persistence
                Context = new(),
                SessionState = sessionId
            }
        };
    }

    public static List<ConversationMessage> ToConversationMessages(
        IEnumerable<Message> messages,
        Guid sessionId)
    {
        return messages
            .Chunk(2)
            .Select(pair => ToConversationMessage(
                pair.FirstOrDefault(m => m.Role == MessageRole.User),
                pair.FirstOrDefault(m => m.Role == MessageRole.Assistant),
                sessionId))
            .ToList();
    }
}