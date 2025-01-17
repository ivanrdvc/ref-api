using Microsoft.SemanticKernel;

using RefApi.Features.Chat.Models;

namespace RefApi.Services;

public interface IChatOptionsService
{
    string GetSystemPrompt(ChatRequestOverrides overrides);
    PromptExecutionSettings GetExecutionSettings(ChatRequestOverrides overrides);
}