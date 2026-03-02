namespace LondonEstate.Models
{
    public class RentHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RentId { get; set; } = Guid.NewGuid();
        public DateTime PaidDate { get; set; }
    }

}
