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

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(x => x.Method)
                   .HasMaxLength(100);

            builder.Property(x => x.PaidAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.HasOne(x => x.Order)
                   .WithMany()
                   .HasForeignKey(x => x.OrderId);
                  // .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.OrderId);
        }
    }
}
