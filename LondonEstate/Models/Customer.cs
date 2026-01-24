using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LondonEstate.Models
{
    public class Customer
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [StringLength(100)]
        public string? Name { get; set; }

        [Required, EmailAddress, StringLength(255)]
        public required string Email { get; set; }

        [Required, StringLength(6, MinimumLength = 1)]
        public required string CountryCode { get; set; }

        [Required, Phone, StringLength(20)]
        public required string PhoneNumber { get; set; }

        public string? Description { get; set; }

        // Inverse navigation for the one-to-many relationship: Customer -> Properties
        [InverseProperty(nameof(Property.Customer))]
        public virtual ICollection<Property> Properties { get; set; } = new HashSet<Property>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}