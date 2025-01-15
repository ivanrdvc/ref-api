using System.Security.Claims;

namespace RefApi.Security;

public class UserContext : IUserContext
{
    private readonly Lazy<bool> _isAuthenticated;
    private readonly Lazy<string?> _userId;
    private readonly Lazy<string?> _email;
    private readonly Lazy<IReadOnlySet<string>> _roles;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        ClaimsPrincipal? user = httpContextAccessor.HttpContext?.User;
        _isAuthenticated = new Lazy<bool>(() => user?.Identity?.IsAuthenticated == true);
        _userId = new Lazy<string?>(() => user?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        _email = new Lazy<string?>(() => user?.FindFirst(ClaimTypes.Email)?.Value);
        _roles = new Lazy<IReadOnlySet<string>>(() =>
            user?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToHashSet()
            ?? []);
    }

    public bool IsAuthenticated => _isAuthenticated.Value;
    public string? UserId => _userId.Value;
    public string? Email => _email.Value;
    public IReadOnlySet<string> Roles => _roles.Value;

    public bool HasRole(string role) => !string.IsNullOrEmpty(role) && Roles.Contains(role);
}