namespace LondonEstate.Models
{
    public class BillType
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
    }
}
