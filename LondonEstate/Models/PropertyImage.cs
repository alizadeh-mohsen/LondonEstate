using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LondonEstate.Data;

namespace LondonEstate.Models
{
    public class PropertyImage
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required Guid PropertyId { get; set; }

        [Required, StringLength(2083)]
        [DataType(DataType.ImageUrl)]
        public required string PicturePath { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property Property { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}