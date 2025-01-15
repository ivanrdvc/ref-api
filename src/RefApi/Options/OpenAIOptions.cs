using System.ComponentModel.DataAnnotations;

namespace RefApi.Options;

public class OpenAIOptions
{
    [Required] public string ChatModelId { get; init; } = string.Empty;

    [Required] public string ApiKey { get; init; } = string.Empty;
}