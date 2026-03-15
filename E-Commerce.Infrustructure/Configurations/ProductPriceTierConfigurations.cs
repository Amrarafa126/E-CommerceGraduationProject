
using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
   
    public class ProductPriceTierConfigurations : IEntityTypeConfiguration<ProductPriceTier>
    {
        public void Configure(EntityTypeBuilder<ProductPriceTier> builder)
        {
            builder.ToTable("ProductPriceTiers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(x => x.MinQuantity)
                   .IsRequired();

            builder.Property(x => x.MaxQuantity)
                   .IsRequired();

            builder.HasOne(x => x.Product)
                   .WithMany(x => x.PriceTiers)
                   .HasForeignKey(x => x.ProductId);
                  // .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ProductId);
        }

    }
}
