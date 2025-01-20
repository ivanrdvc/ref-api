using System.Reflection;

using Asp.Versioning;

using FluentValidation;

using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using RefApi.Common.Behaviors;
using RefApi.Data;
using RefApi.Options;
using RefApi.Services;

namespace RefApi.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddAIServices();
        builder.AddDataServices();

        var services = builder.Services;

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        });

        services.Configure<ClientOptions>(builder.Configuration.GetSection("ClientOptions"));
        services.Configure<AuthClientSetupOptions>(builder.Configuration.GetSection("AuthClientSetupOptions"));
    }

    private static void AddAIServices(this IHostApplicationBuilder builder)
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

        builder.Services.AddSingleton<IAIProviderFactory, AIProviderFactory>();
        builder.Services.AddSingleton<IChatCompletionService>(sp =>
            sp.GetRequiredService<IAIProviderFactory>().CreateChatService());

        builder.Services.AddTransient(sp => new Kernel(sp));
        builder.Services.AddScoped<IChatService, ChatService>();
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