using System.Text.Json.Serialization;

namespace RefApi.Features.Chat.Models;

public record ChatResponse
{
    [JsonPropertyName("message")]
    public ChatMessage? Message { get; init; }

    [JsonPropertyName("delta")]
    public ChatMessage? Delta { get; init; }

    [JsonPropertyName("context")]
    public ResponseContext? Context { get; init; }

    [JsonPropertyName("session_state")]
    public object? SessionState { get; init; }
}