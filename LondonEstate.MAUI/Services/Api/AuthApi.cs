using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LondonEstate.MAUI.Services.Api;

public class AuthApi
{
    private readonly HttpClient _http;

    public AuthApi()
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri("https://your-api-url.com") // CHANGE THIS
        };
    }

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
            throw new Exception("Login failed");
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (result == null || string.IsNullOrWhiteSpace(result.Token))
        {
            throw new Exception("Invalid token response");
        }

        return result.Token;
    }

    public async Task LogoutAsync()
    {
        var token = await SecureStorage.GetAsync("auth_token");

        if (string.IsNullOrWhiteSpace(token))
            return;

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        await _http.PostAsync("/api/auth/logout", null);

        SecureStorage.Remove("auth_token");
    }
}

public class LoginResponse
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
}
