using System.ComponentModel.DataAnnotations;

namespace LondonEstate.Api.Dtos
{
    public class RefreshDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
