using RefApi.Security;

namespace RefApi.Tests.Unit;

public class TestUserContext(string userId) : IUserContext
{
    public bool IsAuthenticated => true;
    public string? UserId => userId;
    public string? Email => "test@example.com";
    public IReadOnlySet<string> Roles => new HashSet<string> { "Admin" };
    public bool HasRole(string role) => Roles.Contains(role);
}