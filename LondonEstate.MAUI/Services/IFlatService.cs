
using LondonEstate.Core.Dtos;

namespace LondonEstate.MAUI.Services
{

    public interface IFlatService
    {
        Task<List<BookingDto>> GetBookings();
    }
}
