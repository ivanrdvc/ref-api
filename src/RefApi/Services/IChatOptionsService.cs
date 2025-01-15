using Microsoft.SemanticKernel.Connectors.OpenAI;

using RefApi.Features.Chat.Models;

namespace RefApi.Services;

public interface IChatOptionsService
{
    OpenAIPromptExecutionSettings GetExecutionSettings(ChatRequestOverrides overrides);
    string GetSystemPrompt(ChatRequestOverrides overrides);
}