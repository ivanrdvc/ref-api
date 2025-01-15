using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using RefApi.Features.Chat.Models;
using RefApi.Options;

namespace RefApi.Services;

public class ChatOptionsService(IOptions<PromptOptions> options) : IChatOptionsService
{
    private readonly PromptOptions _options = options.Value;

    public string GetSystemPrompt(ChatRequestOverrides overrides)
        => overrides.PromptTemplate ?? _options.Prompt;

    public OpenAIPromptExecutionSettings GetExecutionSettings(ChatRequestOverrides overrides) => new()
    {
        Temperature = overrides.Temperature ?? _options.Temperature,
        MaxTokens = _options.MaxTokens,
        TopP = _options.TopP,
        FrequencyPenalty = _options.FrequencyPenalty,
        PresencePenalty = _options.PresencePenalty,
        StopSequences = _options.Stop
    };
}