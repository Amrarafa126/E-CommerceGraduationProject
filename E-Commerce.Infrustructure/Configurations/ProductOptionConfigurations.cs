

using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class ProductOptionConfigurations : IEntityTypeConfiguration<ProductOption>
    {
        public void Configure(EntityTypeBuilder<ProductOption> builder)
        {
            builder.ToTable("ProductOptions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.HasMany(x => x.Values)
                   .WithOne(x => x.Option)
                   .HasForeignKey(x => x.ProductOptionId);
                   //.OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ProductId);
        }

       
    }
}
