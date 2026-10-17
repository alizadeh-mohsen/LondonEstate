using System.ComponentModel.DataAnnotations;

namespace LondonEstate.ViewModels
{
    public class AgreementViewModel
    {
        [Required]
        public decimal? Rent { get; set; }
        [Required]
        public decimal? Deposit { get; set; }
        public required string GuestName { get; set; }
        public required string AccommodationAddress { get; set; }
        public required DateTime CheckInDate { get; set; }
        public required DateTime CheckOutDate { get; set; }
    }
}
