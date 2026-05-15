namespace LondonEstate.Models
{
    public class Flat
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public string? OnlineName { get; set; }
        public string? GuestName { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public bool Empty { get; set; } = true;
        public string? CheckinInstruction { get; set; }
        public string? VisualGuideUrl { get; set; }
        public string? Address { get; set; }
        public string? FlatUrl { get; set; }
        public string? BookingNumber { get; set; }
        public string? ReservationUrl { get; set; }
        public string? Wifi { get; set; }
        public string? Image { get; set; }

    }
}
