namespace RefApi.Configuration;

public class PromptOptions
{
    public string Prompt { get; init; } = string.Empty;
    public int? MaxTokens { get; init; }
    public double Temperature { get; init; }
    public double TopP { get; init; }
    public double FrequencyPenalty { get; init; }
    public double PresencePenalty { get; init; }
    public List<string> Stop { get; init; } = [];
}