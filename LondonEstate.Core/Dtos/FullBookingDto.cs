namespace LondonEstate.Core.Dtos
{
    public class FullBookingDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? OnlineName { get; set; }
        public string? GuestName { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string? BookingNumber { get; set; }
        public string? GuestPhone { get; set; }
        public ICollection<ListingDto> Listings { get; set; }

    }
}
