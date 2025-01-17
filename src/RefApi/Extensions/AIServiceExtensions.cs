using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using RefApi.Constants;
using RefApi.Options;
using RefApi.Services;

namespace RefApi.Extensions;

/// <summary>
/// Extension methods for registering AI services with dependency injection.
/// Configures either OpenAI or Azure OpenAI providers based on AIServiceOptions.Provider setting.
/// </summary>
public static class AIServiceExtensions
{
    public static void AddAIServices(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<AIServiceOptions>()
            .Bind(builder.Configuration.GetSection(nameof(AIServiceOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .AddOptions<PromptOptions>()
            .Bind(builder.Configuration.GetSection(nameof(PromptOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.Configure<AzureAISearchOptions>(
            builder.Configuration.GetSection(nameof(AzureAISearchOptions)));

        builder.Services.AddSingleton<IChatCompletionService>(sp =>
            CreateChatCompletionService(sp.GetRequiredService<IOptions<AIServiceOptions>>().Value));

        builder.Services.AddTransient(sp => new Kernel(sp));
        builder.Services.AddScoped<IChatOptionsService, ChatOptionsService>();
        builder.Services.AddScoped<IChatService, ChatService>();
    }

    private static IChatCompletionService CreateChatCompletionService(AIServiceOptions options)
    {
        return options.Provider.ToLowerInvariant() switch
        {
            AIProviders.OpenAI => new OpenAIChatCompletionService(
                options.OpenAI.ChatModelId,
                options.OpenAI.ApiKey),

            AIProviders.AzureOpenAI => new AzureOpenAIChatCompletionService(
                options.AzureOpenAI.ChatDeploymentName,
                options.AzureOpenAI.Endpoint,
                options.AzureOpenAI.ApiKey),

            _ => throw new InvalidOperationException($"Unsupported provider: {options.Provider}")
        };
    }
}