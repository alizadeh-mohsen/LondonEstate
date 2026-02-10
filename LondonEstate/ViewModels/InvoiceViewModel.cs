using System.ComponentModel.DataAnnotations;

namespace LondonEstate.ViewModels
{
    public class InvoiceViewModel
    {
        public required string CompanyName { get; set; }
        public string? InvoiceNumber { get; set; }
        public required string IssuedTo { get; set; }
        public required string Property { get; set; }
        [Required]
        public decimal? AmountPaid { get; set; }
        public required DateTime PaymentDate { get; set; }
        public required string PaymentMethod { get; set; }
        public required string IssuedBy { get; set; }
        public DateTime Date { get; set; }
        public required DateTime CheckInDate { get; set; }
        public required DateTime CheckOutDate { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public string? ReferenceNumber { get; set; }
    }
}
