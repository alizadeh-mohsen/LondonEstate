
namespace LondonEstate.ViewModels
{
    public class AgreementViewModel
    {
        public required string CompanyName { get; set; }
        public required string SortCode { get; set; }
        public required string Account { get; set; } 
        public required string Rent { get; set; } 
        public required string Deposit { get; set; }
        public required string GuestName { get; set; }
        public required string OwnerName { get; set; }
        public required DateTime Today { get; set; } 
        public required DateTime CheckInDate { get; set; } 
        public required  DateTime CheckOutDate { get; set; } 
    }
}
