#pragma warning disable SKEXP0010
#pragma warning disable AOAI001
using Azure.AI.OpenAI.Chat;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using RefApi.Constants;
using RefApi.Features.Chat.Models;
using RefApi.Options;

namespace RefApi.Services;

public interface IAIProviderFactory
{
    IChatCompletionService CreateChatService();
    PromptExecutionSettings CreateExecutionSettings(ChatRequestOverrides overrides);
}

public class AIProviderFactory(
    IOptions<AIServiceOptions> aiOptions,
    IOptions<PromptOptions> options,
    IOptions<AzureAISearchOptions> searchOptions) : IAIProviderFactory
{
    private readonly AIServiceOptions _aiOptions = aiOptions.Value;
    private readonly PromptOptions _options = options.Value;
    private readonly AzureAISearchOptions _searchOptions = searchOptions.Value;

    public IChatCompletionService CreateChatService() =>
        _aiOptions.Provider.ToLowerInvariant() switch
        {
            AIProviders.OpenAI => new OpenAIChatCompletionService(
                _aiOptions.OpenAI.ChatModelId,
                _aiOptions.OpenAI.ApiKey),

            AIProviders.AzureOpenAI => new AzureOpenAIChatCompletionService(
                _aiOptions.AzureOpenAI.ChatDeploymentName,
                _aiOptions.AzureOpenAI.Endpoint,
                _aiOptions.AzureOpenAI.ApiKey),

            _ => throw new InvalidOperationException($"Unsupported provider: {_aiOptions.Provider}")
        };

    public PromptExecutionSettings CreateExecutionSettings(ChatRequestOverrides overrides)
    {
        var settings = _aiOptions.Provider.ToLowerInvariant() switch
        {
            AIProviders.OpenAI => new OpenAIPromptExecutionSettings(),
            AIProviders.AzureOpenAI => CreateAzureSettings(),
            _ => throw new InvalidOperationException($"Unsupported provider: {_aiOptions.Provider}")
        };

        settings.Temperature = overrides.Temperature ?? _options.Temperature;
        settings.MaxTokens = _options.MaxTokens;
        settings.TopP = _options.TopP;
        settings.FrequencyPenalty = _options.FrequencyPenalty;
        settings.PresencePenalty = _options.PresencePenalty;
        settings.StopSequences = _options.Stop;

        return settings;
    }

    private AzureOpenAIPromptExecutionSettings CreateAzureSettings() =>
        new()
        {
            AzureChatDataSource = new AzureSearchChatDataSource
            {
                Endpoint = new Uri(_searchOptions.Endpoint),
                Authentication = DataSourceAuthentication.FromApiKey(_searchOptions.ApiKey),
                IndexName = _searchOptions.IndexName,
                QueryType = new DataSourceQueryType(_searchOptions.QueryType),
                VectorizationSource = DataSourceVectorizer.FromDeploymentName(_searchOptions.EmbeddingDeploymentName),
                SemanticConfiguration = _searchOptions.SemanticConfiguration,
                Strictness = _searchOptions.Strictness,
                TopNDocuments = _searchOptions.TopNDocuments
            }
        };

    public string GetSystemPrompt(ChatRequestOverrides overrides)
        => overrides.PromptTemplate ?? _options.Prompt;
}