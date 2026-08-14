using AutoMapper;
using LondonEstate.Core.Data;
using LondonEstate.Core.Dtos;
using LondonEstate.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Core.Services
{
    public class FlatService(ApplicationDbContext context, IMapper mapper) : IFlatService
    {

        public async Task<List<FlatDto>> GetAllFlatsAsync()
        {
            var query = from f in context.Flat

                        orderby f.Name
                        select new Flat
                        {
                            Id = f.Id,
                            Name = f.Name,
                        };

            var flats = await query.ToListAsync();
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
        public async Task<FlatDto> CreateFlat(FlatDto flatDto)
        {
            var flat = mapper.Map<Flat>(flatDto);
            await context.Flat.AddAsync(flat);
            await context.SaveChangesAsync();
            return mapper.Map<FlatDto>(flat);
        }
        public async Task<int> UpdateFlat(FlatDto flatDto)
        {
            var flatFromDb = await context.Flat.FindAsync(flatDto.Id);
            if (flatFromDb == null)
                return 0;

            flatFromDb.Name = flatDto.Name;
            flatFromDb.OnlineName = flatDto.OnlineName;
            flatFromDb.Address = flatDto.Address;
            //flatFromDb.FlatUrl = flatDto.FlatUrl;
            flatFromDb.Wifi = flatDto.Wifi;
            flatFromDb.CheckinInstruction = flatDto.CheckinInstruction;
            flatFromDb.Open = flatDto.Open;

            return await context.SaveChangesAsync();
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


        public async Task<List<ListingDto>> GetFlatListingsAsync(Guid id)
        {
            var flats = await context.Listing.Where(l => l.FlatId == id).ToListAsync();
            return mapper.Map<List<ListingDto>>(flats);
        }
        public async Task<ListingDto> CreateListing(ListingDto flatDto)
        {
            var listing = mapper.Map<Listing>(flatDto);
            await context.Listing.AddAsync(listing);
            await context.SaveChangesAsync();
            return mapper.Map<ListingDto>(listing);
        }
        public async Task DeleteListing(Guid id)
        {
            var listing = await context.Listing.FindAsync(id);
            if (listing == null)
            {
                throw new InvalidOperationException("Listing not found");
            }

            context.Listing.Remove(listing);
            await context.SaveChangesAsync();

        }

        public async Task<List<FullBookingDto>> GetBookingsAsync()
        {
            var query = context.Flat
                        .Include(f => f.Listings)
                        .Where(f => f.Open == true)
                        .OrderBy(f => f.Name)
                        .Select(f => new Flat
                        {
                            Id = f.Id,
                            Name = f.Name,
                            OnlineName = f.OnlineName,
                            CheckIn = f.CheckIn,
                            CheckOut = f.CheckOut,
                            GuestName = f.GuestName,
                            Listings = f.Listings
                        });

            var flats = await query.ToListAsync();

            return mapper.Map<List<FullBookingDto>>(flats);
        }
        public async Task<BookingDto> GetBookingAsync(Guid id)
        {
            var query = from f in context.Flat
                        where f.Open == true
                        select new Flat
                        {
                            Id = f.Id,
                            Name = f.Name,
                            OnlineName = f.OnlineName,
                            CheckIn = f.CheckIn,
                            CheckOut = f.CheckOut,
                            GuestName = f.GuestName,
                            GuestPhone = f.GuestPhone,
                            BookingNumber = f.BookingNumber
                        };
            var flat = await query.FirstOrDefaultAsync(f => f.Id == id);

            return flat == null ?
                throw new InvalidOperationException("Flat not found") :
                mapper.Map<BookingDto>(flat);
        }
        public async Task<int> ImportBookingsAsync(BookingImportDto bookingData)
        {
            var flat = await context.Flat
                .FirstOrDefaultAsync(f => f.OnlineName != null && f.OnlineName.ToLower() == bookingData.PropertyName.ToLower());

            if (flat != null)
            {
                flat.GuestName = bookingData.BookerName;
                flat.CheckIn = bookingData.Arrival;
                flat.CheckOut = bookingData.Departure;
                flat.Open = true;
                flat.BookingNumber = bookingData.BookingNumber;
                flat.GuestPhone = bookingData.PhoneNumber;
                return await context.SaveChangesAsync();
            }
            return 0;
        }
        public async Task UpdateBookingAsync(BookingDto flat)
        {
            var flatFromDb = await context.Flat.FindAsync(flat.Id);
            if (flatFromDb == null)
            {
                return;
            }
            flatFromDb.CheckIn = flat.CheckIn;
            flatFromDb.CheckOut = flat.CheckOut;
            //flatFromDb.ReservationUrl = flat.ReservationUrl;
            flatFromDb.BookingNumber = flat.BookingNumber;
            flatFromDb.GuestPhone = flat.GuestPhone;
            flatFromDb.GuestName = flat.GuestName;
            flatFromDb.Open = true;
            await context.SaveChangesAsync();

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
                    //OnlineName = flat.OnlineName,
                    GuestName = flat.GuestName,
                    CheckIn = flat.CheckIn,
                    CheckOut = flat.CheckOut,
                    BookingNumber = flat.BookingNumber,
                    GuestPhone = flat.GuestPhone,
                };
                context.FlatBackup.Add(flatBackup);
            }

            await context.SaveChangesAsync();

        }
        public async Task RestoreAsync()
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
                    //flat.OnlineName = backup.OnlineName;

                    context.Flat.Update(flat);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task<bool> FlatExists(Guid id)
        {
            return await context.Flat.AnyAsync(f => f.Id == id);
        }

        //public async Task<FlatDto> GetFlatByOnlineNameAsync(string onlineName)
        //{
        //    var flat = await context.Flat.FirstOrDefaultAsync(f => f.OnlineName != null && f.OnlineName.ToLower() == onlineName.ToLower());
        //    if (flat == null)
        //    {
        //        return null;
        //    }

        //    return mapper.Map<FlatDto>(flat);
        //}

    }
}
