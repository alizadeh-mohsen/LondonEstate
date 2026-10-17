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

    public FlatService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("ApiClient");
    }

    public async Task<List<BookingDto>> GetBookings()
    {
        var result = await _http.GetFromJsonAsync<List<BookingDto>>("/api/flats/bookings");
        return result ?? new List<BookingDto>();
    }

}



