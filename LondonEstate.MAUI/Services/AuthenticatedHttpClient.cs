namespace LondonEstate.MAUI.Services;

/// <summary>
/// Authenticated HTTP client that automatically handles token injection and refresh
/// Use this for all protected API endpoints that require authentication
/// </summary>
public class AuthenticatedHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public AuthenticatedHttpClient(
        HttpClient httpClient,
        IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    /// <summary>
    /// Get the underlying HttpClient configured with automatic token handling
    /// </summary>
    public HttpClient Client => _httpClient;

    /// <summary>
    /// Get a token-authenticated HttpRequestMessage
    /// </summary>
    public async Task<HttpRequestMessage> CreateAuthenticatedRequestAsync(
        HttpMethod method,
        string requestUri)
    {
        var request = new HttpRequestMessage(method, requestUri);
        var token = await _authService.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return request;
    }
}
