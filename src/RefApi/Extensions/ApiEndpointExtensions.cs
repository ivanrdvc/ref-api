namespace RefApi.Extensions;

/// <summary>
/// Extension methods for configuring common API endpoint response types to avoid repeating response configurations.
/// Provides standard response patterns for GET, POST, DELETE operations with appropriate success and error status codes.
/// </summary>
public static class ApiEndpointExtensions
{
    /// <summary>
    /// Adds common error response types (500, 401, 403)
    /// </summary>
    public static RouteHandlerBuilder WithDefaultResponses(this RouteHandlerBuilder builder)
    {
        return builder
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    /// <summary>
    /// Configures GET endpoint responses including 200 success with type T and 404
    /// </summary>
    public static RouteHandlerBuilder WithGetDefaultResponses<T>(this RouteHandlerBuilder builder)
        where T : class?
    {
        return builder
            .WithDefaultResponses()
            .Produces<T>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Configures POST endpoint responses including 201 created with type T and 400
    /// </summary>
    public static RouteHandlerBuilder WithCreateDefaultResponses<T>(this RouteHandlerBuilder builder)
        where T : class
    {
        return builder
            .WithDefaultResponses()
            .Produces<T>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Configures DELETE endpoint responses including 200 success and 404
    /// </summary>
    public static RouteHandlerBuilder WithDeleteDefaultResponses(this RouteHandlerBuilder builder)
    {
        return builder
            .WithDefaultResponses()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}