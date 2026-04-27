using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class ProductVariantOptionValueConfiguration : IEntityTypeConfiguration<ProductVariantOptionValue>
    {
        public void Configure(EntityTypeBuilder<ProductVariantOptionValue> builder)
        {
            builder.ToTable("ProductVariantOptionValues");
            builder.HasKey(vov => vov.Id);

            builder.HasOne(vov => vov.ProductOptionValue)
                .WithMany(pov => pov.VariantOptionValues)
                .HasForeignKey(vov => vov.ProductOptionValueId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
