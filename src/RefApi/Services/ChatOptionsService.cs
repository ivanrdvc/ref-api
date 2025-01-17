#pragma warning disable SKEXP0010
#pragma warning disable AOAI001

using Azure.AI.OpenAI.Chat;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using RefApi.Constants;
using RefApi.Features.Chat.Models;
using RefApi.Options;

namespace RefApi.Services;

public class ChatOptionsService(
    IOptions<PromptOptions> options,
    IOptions<AIServiceOptions> aiOptions,
    IOptions<AzureAISearchOptions> searchOptions) : IChatOptionsService
{
    private readonly PromptOptions _options = options.Value;
    private readonly AIServiceOptions _aiOptions = aiOptions.Value;
    private readonly AzureAISearchOptions _searchOptions = searchOptions.Value;

    public string GetSystemPrompt(ChatRequestOverrides overrides)
        => overrides.PromptTemplate ?? _options.Prompt;

    public PromptExecutionSettings GetExecutionSettings(ChatRequestOverrides overrides)
    {
        var settings = _aiOptions.Provider?.ToLowerInvariant() switch
        {
            AIProviders.OpenAI => new OpenAIPromptExecutionSettings(),
            AIProviders.AzureOpenAI => CreateAzureSettings(overrides),
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

    private AzureOpenAIPromptExecutionSettings CreateAzureSettings(ChatRequestOverrides overrides)
    {
        var settings = new AzureOpenAIPromptExecutionSettings
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

        return settings;
    }
}