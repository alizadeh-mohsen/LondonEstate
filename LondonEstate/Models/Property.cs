using LondonEstate.Utils.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LondonEstate.Models
{
    public class Property
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK
        [Required]
        public required Guid CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; } = null!;

        [Required, StringLength(250)]
        public required string Address { get; set; }

        [Required]
        public required NumberOfBeds NumberOfBeds { get; set; }

        [Range(1, int.MaxValue)]
        public required decimal SquareMeter { get; set; }

        public float? EstimatedPrice { get; set; }

        public EstimateStatus EstimateStatus { get; set; }

        public string? Description { get; set; }

        [InverseProperty(nameof(PropertyImage.Property))]
        public virtual ICollection<PropertyImage> Images { get; set; } = new HashSet<PropertyImage>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}