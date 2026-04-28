using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class ShippingEventConfiguration : IEntityTypeConfiguration<ShippingEvent>
    {
        public void Configure(EntityTypeBuilder<ShippingEvent> builder)
        {
            builder.ToTable("ShippingEvent");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(30);
            builder.Property(e => e.Description).IsRequired().HasMaxLength(500);
            builder.Property(e => e.Location).HasMaxLength(200);
        }
    }
}
