using Microsoft.AspNetCore.Authorization;

namespace RefApi.Security;

public static class AuthorizationPolicies
{
    public const string RequireAdmin = nameof(RequireAdmin);
    public const string RequireContributor = nameof(RequireContributor);
    public const string WriteAction = nameof(WriteAction);

    public static AuthorizationPolicy DefaultPolicy { get; } = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    public static AuthorizationPolicy WriteActionPolicy { get; } = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim(CustomClaimTypes.Scope, Scopes.Write)
        .Build();

    public static AuthorizationPolicy AdminPolicy { get; } = new AuthorizationPolicyBuilder()
        .RequireRole(nameof(Role.Admin))
        .Build();

    public static AuthorizationPolicy ContributorPolicy { get; } = new AuthorizationPolicyBuilder()
        .RequireRole(nameof(Role.Admin), nameof(Role.Contributor))
        .Build();
}

public static class CustomClaimTypes
{
    public const string Scope = "scope";
}

public static class Scopes
{
    public const string Write = "write";
}