using Asp.Versioning.Builder;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

using RefApi.Configuration;

namespace RefApi.Features.Config;

public static class ConfigApi
{
    public static IEndpointRouteBuilder MapConfigApi(this IEndpointRouteBuilder app)
    {
        var vApi = app.NewVersionedApi("Config");
        var api = vApi.MapGroup("api/v{version:apiVersion}").HasApiVersion(1, 0);

        api.MapGet("/config", GetConfig)
            .WithName("GetConfig")
            .WithOpenApi()
            .Produces<ClientOptions>();

        api.MapGet("/auth_setup", GetAuthSetup)
            .WithName("GetAuthSetup")
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