using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using System.Security.Claims;

using RefApi.Security;

namespace RefApi.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddCustomAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor()
            .AddScoped<IUserContext, UserContext>()
            .AddJwtAuthentication(configuration)
            .AddCustomAuthorization();

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var tenantId = configuration["EntraId:TenantId"]
                       ?? throw new InvalidOperationException("TenantId is not configured");
        var clientId = configuration["EntraId:ClientId"]
                       ?? throw new InvalidOperationException("ClientId is not configured");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://login.microsoftonline.com/{tenantId}";
                options.Audience = $"api://{clientId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        return services;
    }

    private static IServiceCollection AddCustomAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.RequireAdmin, AuthorizationPolicies.AdminPolicy)
            .AddPolicy(AuthorizationPolicies.RequireContributor, AuthorizationPolicies.ContributorPolicy)
            .AddPolicy(AuthorizationPolicies.WriteAction, AuthorizationPolicies.WriteActionPolicy)
            .SetDefaultPolicy(AuthorizationPolicies.DefaultPolicy);

        return services;
    }
}