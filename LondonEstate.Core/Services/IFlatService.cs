using LondonEstate.Core.Dtos;

namespace LondonEstate.Core.Services
{
    public interface IFlatService
    {
        Task<FlatDto> GetFlatAsync(Guid id);
        Task<FlatDto> GetFlatByOnlineNameAsync(string onlineName);
        Task<List<FlatDto>> GetAllFlatsAsync();
        Task<FlatDto> CreateFlat(FlatDto flatDto);
        Task<int> UpdateFlat(FlatDto flatDto);
        Task DeleteFlat(Guid id);
        Task BackupAsync();
        Task RecoverAsync();
        Task<bool> FlatExists(Guid id);
        Task<int> UpdateFlatByImportAsync(BookingImportDto booking);
        Task UpdateFlatForCheckinAsync(FlatDto flat);
    }
}
