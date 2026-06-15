using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class ShippingRateConfiguration : IEntityTypeConfiguration<ShippingRate>
    {
        public void Configure(EntityTypeBuilder<ShippingRate> builder)
        {
            builder.ToTable("ShippingRates");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Zone).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Country).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Cost).HasPrecision(18, 4);
            builder.Property(r => r.Currency).IsRequired().HasMaxLength(3);
            builder.Property(r => r.Method).HasConversion<string>().HasMaxLength(20);
            builder.HasIndex(r => new { r.Country, r.Zone, r.Method, r.IsActive }).IsUnique();
        }
    }
}
