using LondonEstate.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customer { get; set; } = default!;
        public DbSet<Property> Property { get; set; } = default!;
        public DbSet<PropertyImage> PropertyImage { get; set; } = default!;
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ensure Identity model configuration is applied
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                      .HasMaxLength(100);

                entity.Property(c => c.Email)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(c => c.CountryCode)
                      .IsRequired()
                      .HasMaxLength(6);

                entity.Property(c => c.PhoneNumber)
                      .IsRequired()
                      .HasMaxLength(20);

                // Unique index for email
                entity.HasIndex(c => c.Email).IsUnique();

                // One-to-many: Customer -> Properties (cascade delete)
                entity.HasMany(c => c.Properties)
                      .WithOne(p => p.Customer)
                      .HasForeignKey(p => p.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Address)
                      .IsRequired()
                      .HasMaxLength(250);

                entity.Property(p => p.SquareMeter)
                      .IsRequired();

                // Decimal precision for estimated price
                entity.Property(p => p.EstimatedPrice)
                      .HasColumnType("decimal(18,2)");

                // Index on FK to speed lookups
                entity.HasIndex(p => p.CustomerId);

                // One-to-many: Property -> PropertyImage (cascade delete)
                entity.HasMany(p => p.Images)
                      .WithOne(i => i.Property)
                      .HasForeignKey(i => i.PropertyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PropertyImage>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.Property(i => i.PicturePath)
                      .IsRequired()
                      .HasMaxLength(2083);

                // Index on FK for faster queries
                entity.HasIndex(i => i.PropertyId);
            });
        }
    }
}

