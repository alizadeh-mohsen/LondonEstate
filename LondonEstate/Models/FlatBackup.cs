namespace LondonEstate.Models
{
    public class FlatBackup
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public string? OnlineName { get; set; }
        public string? GuestName { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
    }
}
