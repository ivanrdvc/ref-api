using Azure.AI.OpenAI.Chat;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using RefApi.Features.Chat.Models;

namespace RefApi.Configuration;

public interface IAIProviderConfiguration
{
    PromptExecutionSettings GetExecutionSettings(ChatRequestOverrides overrides);
    IChatCompletionService CreateCompletionService();
}

public class OpenAIProviderConfiguration(
    IOptions<PromptOptions> promptOptions,
    IOptions<AIProviderOptions> providerOptions) : IAIProviderConfiguration
{
    private readonly PromptOptions _options = promptOptions.Value;
    private readonly AIProviderOptions _providerOptions = providerOptions.Value;

    public PromptExecutionSettings GetExecutionSettings(ChatRequestOverrides overrides)
    {
        var settings = new OpenAIPromptExecutionSettings
        {
            ChatSystemPrompt = overrides.PromptTemplate ?? _options.Prompt,
            Temperature = overrides.Temperature ?? _options.Temperature,
            MaxTokens = _options.MaxTokens,
            TopP = _options.TopP,
            FrequencyPenalty = _options.FrequencyPenalty,
            PresencePenalty = _options.PresencePenalty,
            StopSequences = _options.Stop
        };

        return settings;
    }

    public IChatCompletionService CreateCompletionService()
    {
        return new OpenAIChatCompletionService(
            _providerOptions.OpenAI.ChatModelId,
            _providerOptions.OpenAI.ApiKey);
    }
}

public class AzureOpenAIProviderConfiguration(
    IOptions<PromptOptions> promptOptions,
    IOptions<AIProviderOptions> providerOptions,
    IOptions<AzureAISearchOptions> searchOptions) : IAIProviderConfiguration
{
    private readonly PromptOptions _options = promptOptions.Value;
    private readonly AzureAISearchOptions _searchOptions = searchOptions.Value;
    private readonly AIProviderOptions _providerOptions = providerOptions.Value;

    public PromptExecutionSettings GetExecutionSettings(ChatRequestOverrides overrides)
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
            },
            ChatSystemPrompt = overrides.PromptTemplate ?? _options.Prompt,
            Temperature = overrides.Temperature ?? _options.Temperature,
            MaxTokens = _options.MaxTokens,
            TopP = _options.TopP,
            FrequencyPenalty = _options.FrequencyPenalty,
            PresencePenalty = _options.PresencePenalty,
            StopSequences = _options.Stop
        };

        return settings;
    }

    public IChatCompletionService CreateCompletionService()
    {
        return new AzureOpenAIChatCompletionService(
            _providerOptions.AzureOpenAI.ChatDeploymentName,
            _providerOptions.AzureOpenAI.Endpoint,
            _providerOptions.AzureOpenAI.ApiKey);
    }
}