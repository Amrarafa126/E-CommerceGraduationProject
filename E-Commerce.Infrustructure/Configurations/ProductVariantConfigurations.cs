using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.SKU).IsRequired().HasMaxLength(100);
        builder.Property(v => v.Price).HasPrecision(18, 4);
        builder.Property(v => v.Barcode).HasMaxLength(100);
        builder.Property(v => v.ImageUrl).HasMaxLength(1000);

        builder.HasIndex(v => new { v.ProductId, v.SKU }).IsUnique();

        builder.HasMany(v => v.OptionValues)
            .WithOne(ov => ov.ProductVariant)
            .HasForeignKey(ov => ov.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
