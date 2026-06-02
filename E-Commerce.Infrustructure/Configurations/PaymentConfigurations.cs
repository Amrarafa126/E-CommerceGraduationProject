using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class PaymentConfigurations : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Amount).HasPrecision(18, 4);
            builder.Property(p => p.AmountRefunded).HasPrecision(18, 4);
            builder.Property(p => p.Currency).IsRequired().HasMaxLength(3);
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(30);
            builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(30);
            builder.Property(p => p.GatewayTransactionId).HasMaxLength(200);
            builder.Property(p => p.GatewayReference).HasMaxLength(200);
            builder.Property(p => p.GatewayRawResponse).HasMaxLength(4000);
            builder.Property(p => p.FailureReason).HasMaxLength(500);
            builder.Property(p => p.CardLast4).HasMaxLength(4);
            builder.Property(p => p.CardBrand).HasMaxLength(30);
            builder.Property(p => p.PaymobToken).HasMaxLength(500);
            builder.Property(p => p.IdempotencyKey).HasMaxLength(100);

            builder.HasIndex(p => p.OrderSubOrderId).IsUnique();
            builder.HasIndex(p => p.GatewayTransactionId);

            builder.HasOne(p => p.OrderSubOrder)
                 .WithOne(s => s.Payment)
                 .HasForeignKey<Payment>(p => p.OrderSubOrderId)
                 .HasPrincipalKey<OrderSubOrder>(s => s.Id)
                 .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
