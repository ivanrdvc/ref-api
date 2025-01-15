using System.Text.Json.Serialization;

namespace RefApi.Features.Chat.Models;

public record ChatRequest
{
    [JsonPropertyName("messages")]
    public required List<ChatMessage> Messages { get; init; } = [];

    [JsonPropertyName("context")]
    public required ChatRequestContext Context { get; init; }

    [JsonPropertyName("session_state")]
    public string? SessionState { get; init; }
}