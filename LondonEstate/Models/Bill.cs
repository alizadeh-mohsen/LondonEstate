namespace LondonEstate.Models
{
    public class Bill
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Notes { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaidDate { get; set; }
        public Guid FlatId { get; set; }
        public Flat? Flat { get; set; }
        public Guid VendorId { get; set; }
        public Vendor? Vendor { get; set; }
        public Guid BillTypeId { get; set; }
        public BillType? BillType { get; set; }

    }
}
