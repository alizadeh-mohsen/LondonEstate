namespace LondonEstate.Core.Models
{
    public class Vendor
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
    }
}
