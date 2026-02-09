using System.ComponentModel.DataAnnotations;

namespace LondonEstate.Models
{
    public class Invoice
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, StringLength(25)]
        public string InvoiceNumber { get; set; }
        [Required, StringLength(50)]
        public string IssuedTo { get; set; }
        [Required]
        public string Property { get; set; }
        [Required]
        public decimal AmountPaid { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        [Required, StringLength(50)]
        public string PaymentMethod { get; set; }
        [Required, StringLength(50)]
        public string IssuedBy { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }
        public string? FileName { get; set; }
    }
}
