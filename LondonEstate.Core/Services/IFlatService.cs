using LondonEstate.Core.Dtos;

namespace LondonEstate.Core.Services
{
    public interface IFlatService
    {
        //Flat
        Task<List<FlatDto>> GetAllFlatsAsync();
        Task<List<FlatDto>> GetAllFlatsInfoAsync();
        Task<List<string>> GetAllFlatAddressAsync();
        Task<FlatDto> GetFlatAsync(Guid id);
        Task<FlatDto> CreateFlat(FlatDto flatDto);
        Task<int> UpdateFlat(FlatDto flatDto);
        Task DeleteFlat(Guid id);

        //Listing
        Task<List<ListingDto>> GetFlatListingsAsync(Guid id);
        Task<ListingDto> CreateListing(ListingDto flatDto);
        Task DeleteListing(Guid id);

        //Booking
        Task<List<BookingDto>> GetBookingsAsync();
        Task<BookingDto> GetBookingAsync(Guid id);
        Task<BookingDto> GetBookingWithListingsAsync(Guid id);
        //Task<FlatDto> GetFlatByOnlineNameAsync(string onlineName);
        Task UpdateBookingAsync(BookingDto flat);


        //Backup and Restore
        Task BackupAsync();
        Task RestoreAsync();

        Task<bool> FlatExists(Guid id);


        Task<int> ImportBookingsAsync(BookingImportDto booking);

    }
}
