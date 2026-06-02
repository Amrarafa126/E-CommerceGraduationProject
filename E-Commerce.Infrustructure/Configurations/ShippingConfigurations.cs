using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class ShippingConfigurations : IEntityTypeConfiguration<Shipping>
    {
        public void Configure(EntityTypeBuilder<Shipping> builder)
        {
            builder.ToTable("Shipments");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.RecipientName).IsRequired().HasMaxLength(200);
            builder.Property(s => s.AddressLine1).IsRequired().HasMaxLength(300);
            builder.Property(s => s.AddressLine2).HasMaxLength(300);
            builder.Property(s => s.City).IsRequired().HasMaxLength(100);
            builder.Property(s => s.State).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Country).IsRequired().HasMaxLength(100);
            builder.Property(s => s.PostalCode).IsRequired().HasMaxLength(20);
            builder.Property(s => s.PhoneNumber).HasMaxLength(20);
            builder.Property(s => s.Method).HasConversion<string>().HasMaxLength(20);
            builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(30);
            builder.Property(s => s.ShippingCost).HasPrecision(18, 4);
            builder.Property(s => s.Currency).IsRequired().HasMaxLength(3);
            builder.Property(s => s.CarrierName).HasMaxLength(100);
            builder.Property(s => s.TrackingNumber).HasMaxLength(100);
            builder.Property(s => s.TrackingUrl).HasMaxLength(500);
            builder.Property(s => s.BostaTrackingNumber).HasMaxLength(100);
            builder.Property(s => s.BostaShipmentId).HasMaxLength(100);

            builder.HasOne(s => s.OrderSubOrder)
              .WithOne(so => so.Shipment)
              .HasForeignKey<Shipping>(s => s.OrderSubOrderId)
              .HasPrincipalKey<OrderSubOrder>(so => so.Id)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Events)
                   .WithOne(e => e.Shipment)
                   .HasForeignKey(e => e.ShipmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
