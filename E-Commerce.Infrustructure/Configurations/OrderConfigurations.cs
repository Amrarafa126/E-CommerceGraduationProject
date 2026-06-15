using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
            builder.Property(o => o.TotalAmount).HasPrecision(18, 4);
            builder.Property(o => o.Currency).IsRequired().HasMaxLength(3);
            builder.Property(o => o.BuyerNotes).HasMaxLength(2000);
            builder.Property(o => o.PoNumber).HasMaxLength(100);
            builder.Property(o => o.OverallStatus).HasConversion<string>().HasMaxLength(30);
            builder.Property(o => o.IdempotencyKey).HasMaxLength(100);
            builder.HasIndex(o => o.OrderNumber).IsUnique();
            builder.HasIndex(o => o.IdempotencyKey)
                .IsUnique()
                .HasFilter("[IdempotencyKey] IS NOT NULL");

            builder.HasOne(o => o.Buyer)
                .WithMany(u => u.Orders).HasForeignKey(o => o.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.SubOrders)
                .WithOne(s => s.Order).HasForeignKey(s => s.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsOne(o => o.ShippingAddress, address =>
            {
                address.ToTable("OrderShippingAddresses");
                address.Property(a => a.RecipientName).IsRequired().HasMaxLength(200);
                address.Property(a => a.PhoneNumber).IsRequired().HasMaxLength(50);
                address.Property(a => a.AddressLine1).IsRequired().HasMaxLength(500);
                address.Property(a => a.AddressLine2).HasMaxLength(500);
                address.Property(a => a.City).IsRequired().HasMaxLength(100);
                address.Property(a => a.State).IsRequired().HasMaxLength(100);
                address.Property(a => a.Country).IsRequired().HasMaxLength(100);
                address.Property(a => a.PostalCode).HasMaxLength(20);
            });
        }
    }
}
