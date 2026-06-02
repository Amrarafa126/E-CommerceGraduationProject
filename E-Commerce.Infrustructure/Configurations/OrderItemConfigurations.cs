using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.ProductName).IsRequired().HasMaxLength(300);
            builder.Property(i => i.ProductMainImageUrl).HasMaxLength(500);
            builder.Property(i => i.ProductDescription).HasMaxLength(2000);
            builder.Property(i => i.ProductCategoryName).HasMaxLength(200);
            builder.Property(i => i.OriginalBasePrice).HasPrecision(18, 4);
            builder.Property(i => i.UnitPrice).HasPrecision(18, 4);
            builder.Property(i => i.VariantSKU).HasMaxLength(100);
            builder.Property(i => i.SellerCompanyName).HasMaxLength(200);
            builder.Property(i => i.ProductVariantName).HasMaxLength(200);
            builder.Property(i => i.NegotiatedUnitPrice).HasPrecision(18, 4);

            builder.HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.ProductVariant)
                .WithMany()
                .HasForeignKey(i => i.ProductVariantId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
}
