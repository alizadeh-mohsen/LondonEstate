using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace LondonEstate.MAUI.Services;

/// <summary>
/// HttpClientHandler that automatically injects JWT tokens into API requests
/// and handles token refresh when expired
/// </summary>
public class AuthenticatedHttpClientHandler : HttpClientHandler
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticatedHttpClientHandler> _logger;

    public AuthenticatedHttpClientHandler(
        IAuthService authService,
        ILogger<AuthenticatedHttpClientHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Intercepts all HTTP requests to automatically add Bearer token
    /// </summary>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get the stored access token
            var token = await _authService.GetTokenAsync();

            if (!string.IsNullOrWhiteSpace(token))
            {
                // Add token to request header
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding token to request: {ex.Message}");
        }

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // If token expired (401), try to refresh and retry
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Received 401 Unauthorized - attempting token refresh");

            try
            {
                var refreshToken = await SecureStorage.GetAsync("refresh_token");

                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    // Try to refresh the token
                    var newToken = await _authService.RefreshTokenAsync(refreshToken);

                    // Retry the original request with new token
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
                else
                {
                    // No refresh token available, user needs to login again
                    await _authService.ClearAuthenticationAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token refresh failed: {ex.Message}");
                // Clear authentication if refresh fails
                await _authService.ClearAuthenticationAsync();
            }
        }

        return response;
    }
}
