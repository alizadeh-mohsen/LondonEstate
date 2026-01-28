using LondonEstate.Utils.Enums;
using System.ComponentModel.DataAnnotations;

namespace LondonEstate.ViewModels
{
    public class PropertyViewModel
    {
        [Required(ErrorMessage = "Required"), StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Required")]
        public NumberOfBeds? NumberOfBeds { get; set; }

        [Required(ErrorMessage = "Required"), Range(1, int.MaxValue)]
        public int? SquareMeter { get; set; }
    }
}
