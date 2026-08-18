using Microsoft.Extensions.Logging;

namespace LondonEstate.MAUI.Services;

/// <summary>
/// Manages automatic token refresh before expiration
/// Runs in the background and refreshes token when it's about to expire
/// </summary>
public class TokenRefreshManager
{
    private readonly IAuthService _authService;
    private readonly ILogger<TokenRefreshManager> _logger;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _refreshTask;

    // Refresh token 5 minutes before it expires (300 seconds)
    private const int REFRESH_BUFFER_SECONDS = 300;

    public TokenRefreshManager(
        IAuthService authService,
        ILogger<TokenRefreshManager> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Start the background token refresh service
    /// </summary>
    public void Start()
    {
        if (_refreshTask != null)
            return;

        _cancellationTokenSource = new CancellationTokenSource();
        _refreshTask = RefreshTokenPeriodicallyAsync(_cancellationTokenSource.Token);

        _logger.LogInformation("Token refresh manager started");
    }

    /// <summary>
    /// Stop the background token refresh service
    /// </summary>
    public async Task StopAsync()
    {
        if (_cancellationTokenSource == null)
            return;

        _cancellationTokenSource.Cancel();

        if (_refreshTask != null)
        {
            try
            {
                await _refreshTask;
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
        }

        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
        _refreshTask = null;

        _logger.LogInformation("Token refresh manager stopped");
    }

    /// <summary>
    /// Background task that periodically checks and refreshes token
    /// </summary>
    private async Task RefreshTokenPeriodicallyAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Check every 60 seconds
                await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);

                // Check if user is still authenticated
                bool isAuthenticated = await _authService.IsAuthenticatedAsync();
                if (!isAuthenticated)
                    continue;

                // Check if token is about to expire
                var expiryTicksStr = await SecureStorage.GetAsync("token_expiry");
                if (string.IsNullOrWhiteSpace(expiryTicksStr) || 
                    !long.TryParse(expiryTicksStr, out var expiryTicks))
                    continue;

                var expiryTime = new DateTime(expiryTicks, DateTimeKind.Utc);
                var timeUntilExpiry = expiryTime - DateTime.UtcNow;

                // If token expires in less than buffer time, refresh it
                if (timeUntilExpiry.TotalSeconds <= REFRESH_BUFFER_SECONDS)
                {
                    await RefreshTokenIfNeededAsync();
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when stopping the service
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in token refresh loop: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Attempt to refresh the token
    /// </summary>
    private async Task RefreshTokenIfNeededAsync()
    {
        try
        {
            var refreshToken = await SecureStorage.GetAsync("refresh_token");

            if (string.IsNullOrWhiteSpace(refreshToken))
                return;

            _logger.LogInformation("Refreshing token before expiration");
            await _authService.RefreshTokenAsync(refreshToken);
            _logger.LogInformation("Token successfully refreshed");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to refresh token: {ex.Message}");
            // If refresh fails, user will be prompted to login on next API call
        }
    }
}
