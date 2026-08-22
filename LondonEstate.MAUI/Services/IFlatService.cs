using LondonEstate.MAUI.Dtos;

namespace LondonEstate.MAUI.Services
{
    /// <summary>
    /// Authentication service interface for handling login, logout, and token management
    /// </summary>
    public interface IFlatService
    {
        public Task<List<BookingDto>> GetBookingsAsync();
    }
}
