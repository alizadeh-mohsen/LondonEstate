using System.ComponentModel.DataAnnotations;

namespace LondonEstate.Utils.Enums
{
    public enum NumberOfBeds
    {
        [Display(Name = "Studio")]
        Studio = 0,

        [Display(Name = "1 Bedroom")]
        OneBedroom = 1,

        [Display(Name = "2 Bedrooms")]
        TwoBedrooms = 2,

        [Display(Name = "3 Bedrooms")]
        ThreeBedrooms = 3,

        [Display(Name = "4 Bedrooms")]
        FourBedrooms = 4,

        [Display(Name = "5 Bedrooms")]
        FiveBedrooms = 5,

        [Display(Name = "6+ Bedrooms")]
        SixPlusMinusBedrooms = 6
    }
}