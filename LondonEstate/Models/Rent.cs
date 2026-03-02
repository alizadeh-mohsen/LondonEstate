namespace LondonEstate.Models
{
    public class Rent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Address { get; set; } = string.Empty;
        public decimal RentAmount { get; set; }
        public short DueDate { get; set; }
        public string? Description { get; set; } = string.Empty;
    }
}
