using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class ProductVariantValueConfigurations : IEntityTypeConfiguration<ProductVariantValue>
    {
        public void Configure(EntityTypeBuilder<ProductVariantValue> builder)
        {
            builder.ToTable("ProductVariantValues");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Variant)
                   .WithMany(x => x.VariantValues)
                   .HasForeignKey(x => x.ProductVariantId);
            //.OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.OptionValue)
                   .WithMany()
                   .HasForeignKey(x => x.ProductOptionValueId);
                   //.OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ProductVariantId);
            builder.HasIndex(x => x.ProductOptionValueId);
        }
    }
}
