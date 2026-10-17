using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LondonEstate.Core.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("IdentityUser")]
        public string UserId { get; set; }

        [Required]
        public string TokenHash { get; set; } // SHA256 hash of the refresh token

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? RevokedDate { get; set; }

        public bool IsRevoked => RevokedDate.HasValue;

        public bool IsExpired => DateTime.UtcNow > ExpiryDate;

        public bool IsValid => !IsRevoked && !IsExpired;
    }
}
