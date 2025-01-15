using RefApi.Data;
using RefApi.Features.Chat;

namespace RefApi.Features.Conversations;

public class Conversation : AuditableEntity
{
    public string Title { get; init; } = string.Empty;
    public long Timestamp { get; init; }
    public string UserId { get; init; } = string.Empty;
    public List<Message> Messages { get; init; } = [];
}