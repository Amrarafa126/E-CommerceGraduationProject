using E_Commerce.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
 
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasOne(x => x.Company)
                   .WithMany(x => x.Users)
                   .HasForeignKey(x => x.CompanyId);
            //.OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Orders)
                   .WithOne(x => x.Buyer)
                   .HasForeignKey(x => x.BuyerId);
            //  .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Reviews)
                   .WithOne(x => x.User)
                   .HasForeignKey(x => x.UserId);
                  // .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.CompanyId);
            builder.HasIndex(x => x.Email);
        }
    }

}
