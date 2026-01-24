using LondonEstate.Utils.Enums;
using System.ComponentModel.DataAnnotations;

namespace LondonEstate.ViewModels
{
    public class PropertyViewModel
    {
        [Required, StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public NumberOfBeds NumberOfBeds { get; set; } = NumberOfBeds.Studio;

        [Required, Range(1, int.MaxValue)]
        public int SquareMeter { get; set; } = 1;
    }
}
