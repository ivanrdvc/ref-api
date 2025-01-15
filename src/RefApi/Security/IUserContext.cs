namespace RefApi.Security;

/// <summary>
/// Provides access to the current user's context and claims.
/// </summary>
public interface IUserContext
{
    bool IsAuthenticated { get; }
    string? UserId { get; }
    string? Email { get; }
    IReadOnlySet<string> Roles { get; }
    bool HasRole(string role);
}