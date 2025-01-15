using System.Text.Json.Serialization;

namespace RefApi.Features.Chat.Models;

public record Thoughts
{
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("description")]
    public required object Description { get; init; }

    [JsonPropertyName("props")]
    public Dictionary<string, string>? Props { get; init; }
}