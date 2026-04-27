
using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{

    public class PriceTierConfiguration : IEntityTypeConfiguration<ProductPriceTier>
    {
        public void Configure(EntityTypeBuilder<ProductPriceTier> builder)
        {
            builder.ToTable("PriceTiers");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.UnitPrice).HasPrecision(18, 4);
        }
    }

}
