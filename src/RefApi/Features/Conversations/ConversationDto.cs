using RefApi.Features.Chat.Models;

namespace RefApi.Features.Conversations;

public record ConversationDto(
    Guid Id,
    string Title,
    long Timestamp
);

public record ConversationMessageDto
{
    public required string User { get; init; }
    public required ChatResponse Response { get; init; }
}