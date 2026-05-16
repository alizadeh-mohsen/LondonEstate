namespace LondonEstate.Models
{
    public class Vendor
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
    }
}
