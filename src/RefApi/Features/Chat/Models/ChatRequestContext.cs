using System.Text.Json.Serialization;

namespace RefApi.Features.Chat.Models;

public record ChatRequestContext
{
    [JsonPropertyName("overrides")]
    public required ChatRequestOverrides Overrides { get; init; }
}