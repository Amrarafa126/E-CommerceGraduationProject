using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Configurations
{
    using E_Commerce.Data.Entity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ProductOptionValueConfigurations : IEntityTypeConfiguration<ProductOptionValue>
    {
        public void Configure(EntityTypeBuilder<ProductOptionValue> builder)
        {
            builder.ToTable("ProductOptionValues");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Value)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.HasOne(x => x.Option)
                   .WithMany(x => x.Values)
                   .HasForeignKey(x => x.ProductOptionId);
                 //  .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ProductOptionId);
        }
    }
}
