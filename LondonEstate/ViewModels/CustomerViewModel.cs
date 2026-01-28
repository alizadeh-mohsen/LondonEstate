using System.ComponentModel.DataAnnotations;

namespace LondonEstate.ViewModels
{
    public class CustomerViewModel
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [Required(ErrorMessage ="Required"), EmailAddress, StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Required"), StringLength(6, MinimumLength = 1)]
        public string CountryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Required"), Phone, StringLength(20)]
        [Display(Name = "Phone")]
        public string Phone { get; set; } = string.Empty;
    }
}
