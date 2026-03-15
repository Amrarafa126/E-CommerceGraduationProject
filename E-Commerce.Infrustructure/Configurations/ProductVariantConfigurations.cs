using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductVariantConfigurations : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SKU)
               .HasMaxLength(100);

        builder.Property(x => x.Price)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.StockQuantity)
               .IsRequired();

        builder.HasOne(x => x.Product)
               .WithMany(x => x.productVariants)
               .HasForeignKey(x => x.ProductId);
        //  .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.OrderItems)
               .WithOne(x => x.ProductVariant)
               .HasForeignKey(x => x.ProductVariantId);
              // .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.SKU);
    }
}