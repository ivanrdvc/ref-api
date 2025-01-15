using System.Text.Json.Serialization;

namespace RefApi.Features.Chat.Models;

public record ChatMessage
{
    [JsonPropertyName("content")]
    public string Content { get; init; } = string.Empty;

    [JsonPropertyName("role")]
    public required string Role { get; init; }
}