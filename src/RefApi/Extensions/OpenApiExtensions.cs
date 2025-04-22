using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

using Scalar.AspNetCore;

namespace RefApi.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddDefaultOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new()
                {
                    Title = "Ref API",
                    Version = "v1",
                    Description = "Ref API using minimal API"
                };
                return Task.CompletedTask;
            });
            
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });


        return services;
    }

    public static IApplicationBuilder UseDefaultOpenApi(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();

        return app;
    }

    private sealed class BearerSecuritySchemeTransformer(
        IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document, 
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
            if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
            {
                var requirements = new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        In = ParameterLocation.Header,
                        BearerFormat = "Json Web Token"
                    }
                };
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = requirements;
            }
        }
    }
}