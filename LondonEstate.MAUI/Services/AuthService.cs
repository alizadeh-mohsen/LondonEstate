using LondonEstate.MAUI.Dtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LondonEstate.MAUI.Services;

/// <summary>
/// Authentication service implementation
/// Handles login, logout, token storage, and token refresh
/// </summary>
public class AuthService(HttpClient _http) : IAuthService
{
    // Secure storage keys
    private const string ACCESS_TOKEN_KEY = "auth_token";
    private const string REFRESH_TOKEN_KEY = "refresh_token";
    private const string TOKEN_EXPIRY_KEY = "token_expiry";

    /// <summary>
    /// Login with email and password
    /// Stores access token and refresh token in secure storage
    /// </summary>
    public async Task<string> LoginAsync(string email, string password)
    {
        var payload = new
        {
            Email = email,
            Password = password
        };

        var response = await _http.PostAsJsonAsync("/api/auth/login", payload);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"Login failed ({(int)response.StatusCode}): {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (result == null || string.IsNullOrWhiteSpace(result.AccessToken))
            throw new Exception("Invalid token response");

        // Store tokens in secure storage
        await SecureStorage.SetAsync(ACCESS_TOKEN_KEY, result.AccessToken);

        if (!string.IsNullOrWhiteSpace(result.RefreshToken))
            await SecureStorage.SetAsync(REFRESH_TOKEN_KEY, result.RefreshToken);

        // Store token expiry time
        if (result.ExpiresIn > 0)
        {
            var expiryTime = DateTime.UtcNow.AddSeconds(result.ExpiresIn);
            await SecureStorage.SetAsync(TOKEN_EXPIRY_KEY, expiryTime.Ticks.ToString());
        }

        return result.AccessToken;
    }

    /// <summary>
    /// Get the stored access token from secure storage
    /// Returns null if no token exists
    /// </summary>
    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(ACCESS_TOKEN_KEY);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error retrieving token: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Check if user is authenticated
    /// Validates that a token exists and hasn't expired
    /// </summary>
    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var token = await SecureStorage.GetAsync(ACCESS_TOKEN_KEY);
            if (string.IsNullOrWhiteSpace(token))
                return false;

            // Check if token has expired
            var expiryTicksStr = await SecureStorage.GetAsync(TOKEN_EXPIRY_KEY);
            if (string.IsNullOrWhiteSpace(expiryTicksStr))
                return true; // No expiry info, assume valid

            if (long.TryParse(expiryTicksStr, out var expiryTicks))
            {
                var expiryTime = new DateTime(expiryTicks, DateTimeKind.Utc);
                if (DateTime.UtcNow >= expiryTime)
                    return false; // Token expired
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Refresh the access token using refresh token
    /// Updates stored tokens
    /// </summary>
    public async Task<string> RefreshTokenAsync(string refreshToken)
    {
        var payload = new { RefreshToken = refreshToken };

        var response = await _http.PostAsJsonAsync("/api/auth/refresh", payload);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"Token refresh failed ({(int)response.StatusCode}): {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (result == null || string.IsNullOrWhiteSpace(result.AccessToken))
            throw new Exception("Invalid refresh response");

        // Update stored tokens
        await SecureStorage.SetAsync(ACCESS_TOKEN_KEY, result.AccessToken);

        if (!string.IsNullOrWhiteSpace(result.RefreshToken))
            await SecureStorage.SetAsync(REFRESH_TOKEN_KEY, result.RefreshToken);

        if (result.ExpiresIn > 0)
        {
            var expiryTime = DateTime.UtcNow.AddSeconds(result.ExpiresIn);
            await SecureStorage.SetAsync(TOKEN_EXPIRY_KEY, expiryTime.Ticks.ToString());
        }

        return result.AccessToken;
    }

    /// <summary>
    /// Logout and remove all authentication data from secure storage
    /// </summary>
    public async Task LogoutAsync()
    {
        var token = await SecureStorage.GetAsync(ACCESS_TOKEN_KEY);
        if (string.IsNullOrWhiteSpace(token))
            return;

        try
        {
            // Notify server of logout
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await _http.SendAsync(request);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Logout error: {ex.Message}");
            // Continue with local cleanup even if server logout fails
        }

        await ClearAuthenticationAsync();
    }

    /// <summary>
    /// Clear all stored authentication data
    /// </summary>
    public async Task ClearAuthenticationAsync()
    {
        try
        {
            SecureStorage.Remove(ACCESS_TOKEN_KEY);
            SecureStorage.Remove(REFRESH_TOKEN_KEY);
            SecureStorage.Remove(TOKEN_EXPIRY_KEY);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error clearing authentication: {ex.Message}");
        }
    }
}

