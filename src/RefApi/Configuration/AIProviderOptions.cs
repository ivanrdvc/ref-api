using System.ComponentModel.DataAnnotations;

namespace RefApi.Configuration;

public enum AIProviderType
{
    OpenAI,
    AzureOpenAI
}

public class AIProviderOptions
{
    [Required] 
    public AIProviderType Provider { get; init; } = AIProviderType.OpenAI;

    public OpenAIOptions OpenAI { get; init; } = new();

    public AzureOpenAIOptions AzureOpenAI { get; init; } = new();
}

public class OpenAIOptions
{
    [Required(AllowEmptyStrings = false)] 
    public string ApiKey { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string ChatModelId { get; init; } = null!;
}

public class AzureOpenAIOptions
{
    [Required(AllowEmptyStrings = false)] 
    public string ApiKey { get; init; } = null!;

    [Required(AllowEmptyStrings = false)] 
    public string Endpoint { get; init; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string ChatDeploymentName { get; init; } = null!;
}