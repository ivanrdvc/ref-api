using System.ComponentModel.DataAnnotations;

namespace RefApi.Options;

public class AIServiceOptions
{
    [Required]
    public string Provider { get; init; } = "OpenAI";

    public OpenAIOptions OpenAI { get; init; } = new();

    public AzureOpenAIOptions AzureOpenAI { get; init; } = new();
}

public class OpenAIOptions
{
    [Required]
    public string ApiKey { get; init; } = string.Empty;

    [Required]
    public string ChatModelId { get; init; } = string.Empty;
}

public class AzureOpenAIOptions
{
    [Required]
    public string ApiKey { get; init; } = string.Empty;

    [Required]
    public string Endpoint { get; init; } = string.Empty;

    [Required]
    public string ChatDeploymentName { get; init; } = string.Empty;
}