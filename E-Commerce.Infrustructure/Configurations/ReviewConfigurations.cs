using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{

    public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.ToTable("ProductReviews");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Title).IsRequired().HasMaxLength(200);
            builder.Property(r => r.Comment).IsRequired().HasMaxLength(2000);
            builder.Property(r => r.SupplierReply).HasMaxLength(1000);

            builder.HasOne(r => r.Product).WithMany()
                .HasForeignKey(r => r.ProductId).
                OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Buyer).WithMany(u => u.Reviews)
                .HasForeignKey(r => r.BuyerId).OnDelete(DeleteBehavior.Restrict);           

            builder.HasIndex(r => new { r.ProductId, r.BuyerId }).IsUnique();

            builder.HasQueryFilter(r => !r.IsDeleted);
        }
    }
}
