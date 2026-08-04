

namespace LondonEstate.ViewModels
{
    public class BookingsViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? OnlineName { get; set; }
        public string? GuestName { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
    }
}
