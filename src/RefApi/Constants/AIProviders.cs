namespace RefApi.Constants;

/// <summary>
/// Defines supported AI providers for completion services.
/// Used for configuring and routing AI service requests.
/// </summary>
public static class AIProviders
{
    /// <summary>
    /// Direct OpenAI API provider using API key authentication.
    /// Requires OpenAI.ApiKey and OpenAI.ChatModelId configuration.
    /// </summary>
    public const string OpenAI = "openai";

    /// <summary>
    /// Azure-hosted OpenAI service provider.
    /// Requires AzureOpenAI.Endpoint, AzureOpenAI.ApiKey and AzureOpenAI.ChatDeploymentName configuration.
    /// </summary>
    public const string AzureOpenAI = "azureopenai";
}