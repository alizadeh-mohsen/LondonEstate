namespace LondonEstate.Core.Dtos
{
    public class BookingImportDto
    {
        public string PropertyName { get; set; } = string.Empty;
        public string? BookerName { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
        public string BookingNumber { get; set; }
        public string PhoneNumber { get; set; }

    }
}
