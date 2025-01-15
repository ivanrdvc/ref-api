namespace RefApi.Controllers;

/// <summary>
/// Contains constants used for API routing and versioning.
/// </summary>
public static class ApiConstants
{
    /// <summary>
    /// The base route for API endpoints, with a placeholder for the API version.
    /// </summary>
    private const string BaseRouteWithVersion = "api/v{version:apiVersion}/";

    /// <summary>
    /// Placeholder for the controller name.
    /// </summary>
    private const string ControllerPlaceholder = "[controller]";

    /// <summary>
    /// Standard route pattern used across all API controllers.
    /// Combines <see cref="BaseRouteWithVersion"/> and <see cref="ControllerPlaceholder"/>.
    /// </summary>
    public const string StandardRoute = BaseRouteWithVersion + ControllerPlaceholder;
}

/// <summary>
/// Contains constants for defining supported API versions.
/// </summary>
public static class ApiVersions
{
    /// <summary>
    /// The initial version of the API.
    /// </summary>
    public const string V1 = "1.0";
}