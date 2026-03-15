using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{

public class ReviewConfigurations : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(x => x.ReviewId);

        builder.Property(x => x.Rating)
               .IsRequired();

        builder.Property(x => x.Comment)
               .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("GETDATE()");

            builder.HasOne(x => x.Product)
                   .WithMany(x => x.Reviews)
                   .HasForeignKey(x => x.ProductId);
            //.OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User)
                   .WithMany(x => x.Reviews)
                   .HasForeignKey(x => x.UserId);
              // .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.ProductId, x.UserId }).IsUnique();
        }
}
}
