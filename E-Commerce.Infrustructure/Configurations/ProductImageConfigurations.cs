using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
 
    public class ProductImageConfigurations : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ImageUrl)
                   .HasMaxLength(1000)
                   .IsRequired();

            builder.HasOne(x => x.Product)
                   .WithMany(x => x.Images)
                   .HasForeignKey(x => x.ProductId);

            builder.HasIndex(x => x.ProductId);
        }

    }

}
