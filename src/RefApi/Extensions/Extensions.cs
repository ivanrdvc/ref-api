using System.Reflection;

using Asp.Versioning;

using FluentValidation;

using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using RefApi.Common;
using RefApi.Configuration;
using RefApi.Data;
using RefApi.Features.Chat.Commands;
using RefApi.Features.Chat.Models;
using RefApi.Features.Conversations;
using RefApi.Features.Conversations.Commands;
using RefApi.Features.Conversations.Queries;

namespace RefApi.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddAIServices();
        builder.AddDataServices();

        var services = builder.Services;

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // TODO: Refactor repetitive IRequestHandler
        services.AddScoped<IRequestHandler<ChatCommand, ChatResponse>, ChatCommandHandler>();
        services.AddScoped<IRequestHandler<ChatCommand, ChatResponse>, ChatCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteConversationCommand, bool>, DeleteConversationCommandHandler>();
        services.AddScoped<IRequestHandler<SaveConversationCommand, bool>, SaveConversationCommandHandler>();
        services.AddScoped<IStreamRequestHandler<StreamChatCommand, ChatResponse>, StreamChatCommandHandler>();
        services.AddScoped<IRequestHandler<GetConversationQuery, List<ConversationMessageDto>?>, GetConversationQueryHandler>();
        services.AddScoped<IRequestHandler<GetConversationsQuery, GetConversationsResponse>, GetConversationsQueryHandler>();

        services.Configure<ClientOptions>(builder.Configuration.GetSection("ClientOptions"));
        services.Configure<AuthClientSetupOptions>(builder.Configuration.GetSection("AuthClientSetupOptions"));
    }

    private static void AddAIServices(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<AIProviderOptions>()
            .Bind(builder.Configuration.GetSection(nameof(AIProviderOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .AddOptions<PromptOptions>()
            .Bind(builder.Configuration.GetSection(nameof(PromptOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.Configure<AzureAISearchOptions>(
            builder.Configuration.GetSection(nameof(AzureAISearchOptions)));

        var providerType = builder.Configuration.GetValue<AIProviderType>($"{nameof(AIProviderOptions)}:Provider");

        switch (providerType)
        {
            case AIProviderType.OpenAI:
                builder.Services.AddSingleton<IAIProviderConfiguration, OpenAIProviderConfiguration>();
                break;

            case AIProviderType.AzureOpenAI:
                builder.Services.AddSingleton<IAIProviderConfiguration, AzureOpenAIProviderConfiguration>();
                break;

            default:
                throw new InvalidOperationException($"Unsupported provider: {providerType}");
        }

        builder.Services.AddSingleton<IChatCompletionService>(sp =>
            sp.GetRequiredService<IAIProviderConfiguration>().CreateCompletionService());

        builder.Services.AddTransient(sp => new Kernel(sp));
    }

    private static void AddDataServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
    }

    public static IServiceCollection AddDefaultCorsPolicy(this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

    public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    public static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
            };
        });

        return services;
    }
}