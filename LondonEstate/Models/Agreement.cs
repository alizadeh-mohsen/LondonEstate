using System.ComponentModel.DataAnnotations;

namespace LondonEstate.Models
{
    public class Agreement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, StringLength(20)]
        public required string SortCode { get; set; }
        [Required, StringLength(20)]
        public required string Account { get; set; }
        [Required]
        public decimal Rent { get; set; }
        [Required,]
        public decimal Deposit { get; set; }
        [Required, StringLength(50)]
        public string GuestName { get; set; }
        [Required, StringLength(50)]
        public string OwnerName { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }
        public string? FileName { get; set; }

    }
}
