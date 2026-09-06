using AutoMapper;
using LondonEstate.Core.Data;
using LondonEstate.Core.Dtos;
using LondonEstate.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Core.Services
{
    public class ReminderService(ApplicationDbContext context, IMapper mapper) : IReminderService
    {
        public async Task<List<ReminderDto>> GetAllRemindersAsync()
        {
            var reminders = await context.Reminder
                .OrderBy(r => r.Due)
                .Select(r => new Reminder
                {
                    Id = r.Id,
                    Due = r.Due,
                    Note = r.Note
                })
                .ToListAsync();

            return mapper.Map<List<ReminderDto>>(reminders);
        }


        public async Task<ReminderDto> GetReminderAsync(Guid id)
        {
            var reminder = await context.Reminder.FindAsync(id);

            if (reminder == null)
                throw new InvalidOperationException("Reminder not found");

            return mapper.Map<ReminderDto>(reminder);
        }

        public async Task<ReminderDto> CreateReminderAsync(ReminderDto reminderDto)
        {
            var reminder = mapper.Map<Reminder>(reminderDto);

            await context.Reminder.AddAsync(reminder);
            await context.SaveChangesAsync();

            return mapper.Map<ReminderDto>(reminder);
        }

        public async Task<int> UpdateReminderAsync(ReminderDto reminderDto)
        {
            var reminderFromDb = await context.Reminder.FindAsync(reminderDto.Id);

            if (reminderFromDb == null)
                return 0;

            reminderFromDb.Due = reminderDto.Due;
            reminderFromDb.Note = reminderDto.Note;

            return await context.SaveChangesAsync();
        }

        public async Task DeleteReminderAsync(Guid id)
        {
            var reminder = await context.Reminder.FindAsync(id);

            if (reminder == null)
                throw new InvalidOperationException("Reminder not found");

            context.Reminder.Remove(reminder);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ReminderExistsAsync(Guid id)
        {
            return await context.Reminder.AnyAsync(r => r.Id == id);
        }
    }

}
