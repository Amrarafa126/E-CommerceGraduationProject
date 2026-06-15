using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class OrderSubOrderConfiguration : IEntityTypeConfiguration<OrderSubOrder>
    {
        public void Configure(EntityTypeBuilder<OrderSubOrder> builder)
        {
            builder.ToTable("OrderSubOrders");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.SubOrderNumber).IsRequired().HasMaxLength(50);
            builder.Property(s => s.SubTotal).HasPrecision(18, 4);
            builder.Property(s => s.ShippingCost).HasPrecision(18, 4);
            builder.Property(s => s.TaxAmount).HasPrecision(18, 4);
            builder.Property(s => s.TotalAmount).HasPrecision(18, 4);
            builder.Property(s => s.Currency).IsRequired().HasMaxLength(3);
            builder.Property(s => s.CancellationReason).HasMaxLength(500);
            builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(s => s.ShippingMethod).HasConversion<string>().HasMaxLength(20);
            builder.Property(s => s.PaymentTerms).HasMaxLength(50);
            builder.Property(s => s.DepositAmount).HasPrecision(18, 4);
            builder.Property(s => s.BalanceDue).HasPrecision(18, 4);
            builder.HasIndex(s => s.SubOrderNumber).IsUnique();

            builder.HasOne(s => s.SellerCompany)
                .WithMany().HasForeignKey(s => s.SellerCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Items)
                .WithOne(i => i.OrderSubOrder).HasForeignKey(i => i.OrderSubOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.StatusHistory)
                .WithOne(h => h.OrderSubOrder).HasForeignKey(h => h.OrderSubOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
