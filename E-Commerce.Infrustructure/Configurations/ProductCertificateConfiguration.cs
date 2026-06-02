using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class ProductCertificateConfiguration : IEntityTypeConfiguration<ProductCertificate>
    {
        public void Configure(EntityTypeBuilder<ProductCertificate> builder)
        {
            builder.ToTable("ProductCertificates");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Url).IsRequired().HasMaxLength(1000);
            builder.Property(c => c.OriginalFileName).IsRequired().HasMaxLength(255);
            builder.Property(c => c.ContentType).IsRequired().HasMaxLength(100);
            builder.Property(c => c.IssuedBy).HasMaxLength(200);

            builder.HasOne(c => c.Product)
                .WithMany(p => p.Certificates)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
