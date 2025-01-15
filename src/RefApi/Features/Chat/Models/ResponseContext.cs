using System.Text.Json.Serialization;

namespace RefApi.Features.Chat.Models;

public record ResponseContext
{
    [JsonPropertyName("data_points")]
    public List<string> DataPoints { get; init; } = [];

    [JsonPropertyName("followup_questions")]
    public List<string>? FollowupQuestions { get; init; }

    [JsonPropertyName("thoughts")]
    public List<Thoughts> Thoughts { get; init; } = [];
}