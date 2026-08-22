using LondonEstate.Core.Dtos;
using System.Net.Http.Json;

namespace LondonEstate.MAUI.Services;

/// <summary>
/// Authentication service implementation
/// Handles login, logout, token storage, and token refresh
/// </summary>
public class FlatService : IFlatService
{
    private readonly HttpClient _http;

    public FlatService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<BookingDto>> GetBookingsAsync()
    {
        return await _http.GetFromJsonAsync<List<BookingDto>>("api/bookings");
    }
}



