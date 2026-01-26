using LondonEstate.Utils.Enums;
using System.ComponentModel.DataAnnotations;

namespace LondonEstate.ViewModels
{
    public class PropertyViewModel
    {
        [Required, StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Number Of Beds field is required.")]
        public NumberOfBeds? NumberOfBeds { get; set; }

        [Required(ErrorMessage ="The size field is required"), Range(1, int.MaxValue)]
        public int? SquareMeter { get; set; } 
    }
}
