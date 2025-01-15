namespace RefApi.Options;

/// <summary>
/// Configuration options for controlling UI feature visibility in the Azure OpenAI Search demo application.
/// </summary>
public class ClientOptions
{
    public bool ShowGpt4VOptions { get; init; }
    public bool ShowSemanticRankerOption { get; init; }
    public bool ShowVectorOption { get; init; }
    public bool ShowUserUpload { get; init; }
    public bool ShowLanguagePicker { get; init; }
    public bool ShowSpeechInput { get; init; }
    public bool ShowSpeechOutputBrowser { get; init; }
    public bool ShowSpeechOutputAzure { get; init; }
    public bool ShowChatHistoryBrowser { get; init; }
    public bool ShowChatHistoryCosmos { get; init; }
    public bool ShowChatHistoryCustomDb { get; init; }
}

public class AuthClientSetupOptions
{
    public bool UseLogin { get; init; }
    public bool RequireAccessControl { get; init; }
    public bool EnableUnauthenticatedAccess { get; init; }
    public MsalConfig MsalConfig { get; init; } = new();
    public LoginRequest LoginRequest { get; init; } = new();
    public TokenRequest TokenRequest { get; init; } = new();
}

public class MsalConfig
{
    public AuthConfig Auth { get; init; } = new();
    public CacheConfig Cache { get; init; } = new();
}

public class AuthConfig
{
    public string ClientId { get; init; } = string.Empty;
    public string Authority { get; init; } = string.Empty;
    public string RedirectUri { get; init; } = string.Empty;
    public string PostLogoutRedirectUri { get; init; } = string.Empty;
    public bool NavigateToLoginRequestUrl { get; init; }
}

public class CacheConfig
{
    public string CacheLocation { get; init; } = "localStorage";
    public bool StoreAuthStateInCookie { get; init; }
}

public class LoginRequest
{
    public string[] Scopes { get; init; } = [];
}

public class TokenRequest
{
    public string[] Scopes { get; init; } = [];
}