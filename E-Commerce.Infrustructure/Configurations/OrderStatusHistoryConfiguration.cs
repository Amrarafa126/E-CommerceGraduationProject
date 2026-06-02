using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
        {
            builder.ToTable("OrderStatusHistory");
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(h => h.Note).IsRequired().HasMaxLength(500);
        }
    }
}
