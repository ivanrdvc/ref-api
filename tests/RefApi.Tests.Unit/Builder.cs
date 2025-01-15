using RefApi.Features.Chat;
using RefApi.Features.Chat.Models;
using RefApi.Features.Conversations;

namespace RefApi.Tests.Unit;

public static class Builder
{
    public static List<Message> CreateDomainMessagePair(string userMessage, string assistantResponse)
    {
        return
        [
            new() { Role = MessageRole.User, Content = userMessage },
            new() { Role = MessageRole.Assistant, Content = assistantResponse }
        ];
    }

    public static List<ConversationMessage> CreateUserAssistantPairs(params (string User, string Assistant)[] pairs)
    {
        return pairs.Select(pair => new ConversationMessage
        {
            User = pair.User,
            Response = new ChatResponse
            {
                Message = new ChatMessage { Content = pair.Assistant, Role = MessageRole.Assistant }
            }
        }).ToList();
    }
}