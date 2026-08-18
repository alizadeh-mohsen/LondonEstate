using LondonEstate.MAUI.Dtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LondonEstate.MAUI.Services;

public class AuthService(HttpClient _http) : IAuthService
{
    //private readonly HttpClient _http;

    //public AuthApi()
    //{
    //    _http = new HttpClient
    //    {
    //        BaseAddress = new Uri("http://localhost:5002") // CHANGE THIS
    //    };
    //}

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
            // log body and response.StatusCode
            throw new Exception($"Login failed ({(int)response.StatusCode}): {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (result == null || string.IsNullOrWhiteSpace(result.AccessToken))
            throw new Exception("Invalid token response");

        return result.AccessToken;
    }

    public async Task LogoutAsync()
    {
        var token = await SecureStorage.GetAsync("auth_token");
        if (string.IsNullOrWhiteSpace(token))
            return;

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await _http.SendAsync(request);

        SecureStorage.Remove("auth_token");
    }
}

