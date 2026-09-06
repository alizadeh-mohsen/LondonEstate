using LondonEstate.Core.Dtos;

namespace LondonEstate.Core.Services
{
    public interface IReminderService
    {
        Task<List<ReminderDto>> GetAllRemindersAsync();
        Task<ReminderDto> GetReminderAsync(Guid id);
        Task<ReminderDto> CreateReminderAsync(ReminderDto reminderDto);
        Task<int> UpdateReminderAsync(ReminderDto reminderDto);
        Task DeleteReminderAsync(Guid id);
        Task<bool> ReminderExistsAsync(Guid id);
    }

}
