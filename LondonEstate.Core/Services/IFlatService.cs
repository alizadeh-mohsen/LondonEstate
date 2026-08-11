using LondonEstate.Core.Dtos;

namespace LondonEstate.Core.Services
{
    public interface IFlatService
    {
        Task<List<FlatDto>> GetAllFlatsAsync(); 
        Task<FlatDto> GetFlatAsync(Guid id);


        Task<List<BookingDto>> GetBookingsAsync(); 
        Task<BookingDto> GetBookingAsync(Guid id);
        //Task<FlatDto> GetFlatByOnlineNameAsync(string onlineName);
        Task UpdateBookingAsync(BookingDto flat);

        Task<FlatDto> CreateFlat(FlatDto flatDto);
        Task<int> UpdateFlat(FlatDto flatDto);
        Task DeleteFlat(Guid id);
        
        
        Task BackupAsync();
        Task RestoreAsync();
        
        Task<bool> FlatExists(Guid id);
        
        
        Task<int> ImportBookingsAsync(BookingImportDto booking);
        
    }
}
