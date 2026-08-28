using System.ComponentModel.DataAnnotations;

namespace LondonEstate.ViewModels
{
    public class InvoiceViewModel
    {
        public string? InvoiceNumber { get; set; }
        public required string IssuedTo { get; set; }
        public required string Property { get; set; }
        [Required]
        public required string? AmountPaid { get; set; }
        public required DateTime PaymentDate { get; set; }
        public required string IssuedBy { get; set; }
        public required DateTime CheckInDate { get; set; }
        public required DateTime CheckOutDate { get; set; }
        public string? ReferenceNumber { get; set; }
    }
}
