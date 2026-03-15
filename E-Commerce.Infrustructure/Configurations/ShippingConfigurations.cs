using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
   
    public class ShippingConfigurations : IEntityTypeConfiguration<Shipping>
    {
        public void Configure(EntityTypeBuilder<Shipping> builder)
        {
            builder.ToTable("Shippings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ShippingCompany)
                   .HasMaxLength(200);

            builder.Property(x => x.TrackingNumber)
                   .HasMaxLength(200);

            builder.Property(x => x.ShippingDate)
                   .IsRequired();

            builder.HasOne(x => x.Order)
                   .WithOne()
                   .HasForeignKey<Shipping>(x => x.OrderId);
                 //  .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.OrderId);
        }
    }
}
