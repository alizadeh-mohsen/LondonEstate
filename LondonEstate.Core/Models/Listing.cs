namespace LondonEstate.Core.Models
{
    public class Listing
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid FlatId { get; set; }
        public string? OnlineName { get; set; }
    }
}
