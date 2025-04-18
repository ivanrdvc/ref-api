using Asp.Versioning.Builder;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

using RefApi.Options;

namespace RefApi.Features.Config;

public static class ConfigApi
{
    public static IEndpointRouteBuilder MapConfigApiV1(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
    {
        app.MapGet("api/v{version:apiVersion}/config", GetConfig)
            .WithApiVersionSet(versionSet)
            .WithOpenApi()
            .Produces<ClientOptions>();

        app.MapGet("api/v{version:apiVersion}/auth_setup", GetAuthSetup)
            .WithApiVersionSet(versionSet)
            .WithOpenApi()
            .Produces<AuthClientSetupOptions>();

        return app;
    }

    private static Results<Ok<ClientOptions>, ProblemHttpResult> GetConfig(IOptions<ClientOptions> options)
    {
        return TypedResults.Ok(options.Value);
    }

    private static Results<Ok<AuthClientSetupOptions>, ProblemHttpResult> GetAuthSetup(
        IOptions<AuthClientSetupOptions> options)
    {
        return TypedResults.Ok(options.Value);
    }
}