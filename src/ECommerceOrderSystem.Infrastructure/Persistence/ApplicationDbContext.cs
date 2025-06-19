using Microsoft.EntityFrameworkCore;
using ECommerceOrderSystem.Domain.Entities;
using ECommerceOrderSystem.Domain.ValueObjects;
using ECommerceOrderSystem.Domain.Enums;

namespace ECommerceOrderSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Order entity configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.UserId)
                    .HasConversion(
                        v => v.Value,
                        v => new UserId(v))
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.ProductId)
                    .HasConversion(
                        v => v.Value,
                        v => new ProductId(v))
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.Property(e => e.PaymentMethod)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.UpdatedAt);

                entity.Property(e => e.ProcessedAt);

                // Indexes for performance
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
            });
        }
    }
}