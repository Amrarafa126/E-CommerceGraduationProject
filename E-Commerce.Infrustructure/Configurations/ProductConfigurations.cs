
using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Table Name
            builder.ToTable("Products");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties

            builder.Property(x => x.Name)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.Description)
                   .HasMaxLength(2000);

            builder.Property(x => x.MinimumOrderQuantity)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd();

            #region Relationships

            builder.HasOne(x => x.Company)
                       .WithMany(x => x.Products)
                       .HasForeignKey(x => x.CompanyId);
            // .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Category)
                   .WithMany(x => x.Products)
                   .HasForeignKey(x => x.CategoryId);
            // .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Images)
                   .WithOne(x => x.Product)
                   .HasForeignKey(x => x.ProductId);
                   //.OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.PriceTiers)
                   .WithOne(x => x.Product)
                   .HasForeignKey(x => x.ProductId);
            //.OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Reviews)
                   .WithOne(x => x.Product)
                   .HasForeignKey(x => x.ProductId);
            // .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.productVariants)
                   .WithOne(x => x.Product)
                   .HasForeignKey(x => x.ProductId);
                  // .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.CompanyId);
            builder.HasIndex(x => x.CategoryId);

            #endregion
        }

    }
}