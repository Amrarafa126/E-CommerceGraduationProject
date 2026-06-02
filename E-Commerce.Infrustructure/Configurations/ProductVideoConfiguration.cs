using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class ProductVideoConfiguration : IEntityTypeConfiguration<ProductVideo>
    {
        public void Configure(EntityTypeBuilder<ProductVideo> builder)
        {
            builder.ToTable("ProductVideos");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Url).IsRequired().HasMaxLength(1000);
            builder.Property(v => v.Title).HasMaxLength(200);
            builder.Property(v => v.ThumbnailUrl).HasMaxLength(1000);

            builder.HasOne(v => v.Product)
                .WithMany(p => p.Videos)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(v => !v.IsDeleted);
        }
    }
}
