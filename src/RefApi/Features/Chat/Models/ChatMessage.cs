using System.ComponentModel;
using System.Text.Json.Serialization;

namespace RefApi.Features.Chat.Models;

public record ChatMessage
{
    [JsonPropertyName("content")]
    [DefaultValue("hello")]
    public string Content { get; init; } = string.Empty;

    [JsonPropertyName("role")]
    [DefaultValue("user")]
    public required string Role { get; init; }
}