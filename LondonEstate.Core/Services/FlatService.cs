using AutoMapper;
using LondonEstate.Core.Data;
using LondonEstate.Core.Dtos;
using LondonEstate.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Core.Services
{
    public class FlatService(ApplicationDbContext context, IMapper mapper) : IFlatService
    {
        public async Task<FlatDto> CreateFlat(FlatDto flatDto)
        {
            var flat = mapper.Map<Flat>(flatDto);
            await context.Flat.AddAsync(flat);
            await context.SaveChangesAsync();
            return mapper.Map<FlatDto>(flat);
        }

        public async Task DeleteFlat(Guid id)
        {
            var flat = await context.Flat.FindAsync(id);
            if (flat == null)
            {
                throw new InvalidOperationException("Flat not found");
            }

            context.Flat.Remove(flat);
            await context.SaveChangesAsync();

        }

        public async Task<List<FlatDto>> GetAllFlatsAsync()
        {
            var flats = await context.Flat.ToListAsync();
            return mapper.Map<List<FlatDto>>(flats);
        }

        public async Task<FlatDto> GetFlatAsync(Guid id)
        {
            var flat = await context.Flat.FindAsync(id);
            if (flat == null)
            {
                throw new InvalidOperationException("Flat not found");
            }

            return mapper.Map<FlatDto>(flat);
        }
        public async Task<FlatDto> GetFlatByOnlineNameAsync(string onlineName)
        {
            var flat = await context.Flat.FirstOrDefaultAsync(f => f.OnlineName != null && f.OnlineName.ToLower() == onlineName.ToLower());
            if (flat == null)
            {
                throw new InvalidOperationException("Flat not found");
            }

            return mapper.Map<FlatDto>(flat);
        }

        public async Task<int> UpdateFlat(FlatDto flatDto)
        {
            var flat = await context.Flat.FindAsync(flatDto.Id);
            mapper.Map(flatDto, flat);
            return await context.SaveChangesAsync();

        }
        public async Task<int> UpdateFlatsFromImportAsync(List<BookingImportDto> bookingData)
        {
            int updatedCount = 0;

            //backup existing flats before updating

            foreach (var booking in bookingData)
            {
                var flat = await context.Flat
                    .FirstOrDefaultAsync(f => f.OnlineName != null && f.OnlineName.ToLower() == booking.PropertyName.ToLower());

                if (flat != null)
                {
                    flat.GuestName = booking.BookerName;
                    flat.CheckIn = booking.Arrival;
                    flat.CheckOut = booking.Departure;
                    flat.Open = true;
                    flat.BookingNumber = booking.BookingNumber;
                    flat.GuestPhone = booking.PhoneNumber;
                    flat.TotalPayment = booking.TotalPayment;

                    context.Flat.Update(flat);
                    updatedCount++;
                }
            }

            if (updatedCount > 0)
            {
                await context.SaveChangesAsync();
            }

            return updatedCount;
        }

        public async Task BackupAsync()
        {

            var existingFlats = await context.Flat.ToListAsync();
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE FlatBackup");

            foreach (var flat in existingFlats)
            {
                var flatBackup = new FlatBackup
                {
                    Id = flat.Id,
                    Name = flat.Name,
                    OnlineName = flat.OnlineName,
                    GuestName = flat.GuestName,
                    CheckIn = flat.CheckIn,
                    CheckOut = flat.CheckOut,
                    BookingNumber = flat.BookingNumber,
                    GuestPhone = flat.GuestPhone,
                    TotalPayment = flat.TotalPayment == null ? 0 : flat.TotalPayment.Value,
                };
                context.FlatBackup.Add(flatBackup);
            }

            await context.SaveChangesAsync();

        }

        public async Task RecoverAsync()
        {
            var backupFlats = await context.FlatBackup.ToListAsync();

            // Restore all flats from backup
            foreach (var backup in backupFlats)
            {
                var flat = await context.Flat.FirstOrDefaultAsync(f => f.Id == backup.Id);

                if (flat != null)
                {
                    flat.GuestName = backup.GuestName;
                    flat.GuestPhone = backup.GuestPhone;
                    flat.BookingNumber = backup.BookingNumber;
                    flat.CheckIn = backup.CheckIn;
                    flat.CheckOut = backup.CheckOut;
                    flat.Name = backup.Name;
                    flat.OnlineName = backup.OnlineName;
                    flat.TotalPayment = backup.TotalPayment;

                    context.Flat.Update(flat);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task<bool> FlatExists(Guid id)
        {
            return await context.Flat.AnyAsync(f => f.Id == id);
        }
    }
}
