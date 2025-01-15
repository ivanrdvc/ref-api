using RefApi.Data;
using RefApi.Features.Conversations;

namespace RefApi.Features.Chat;

public class Message : AuditableEntity
{
    public Guid ConversationId { get; init; }
    public string Role { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public virtual Conversation Conversation { get; init; } = null!;
}

public static class MessageRole
{
    public const string System = "system";
    public const string User = "user";
    public const string Assistant = "assistant";
    public const string Tool = "tool";
}