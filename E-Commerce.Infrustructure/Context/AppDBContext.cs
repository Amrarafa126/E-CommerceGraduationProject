using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Data.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace E_Commerce.Infrustructure.Context
{
    public class AppDBContext(DbContextOptions<AppDBContext> options)
      : IdentityDbContext<User,Role,Guid,IdentityUserClaim<Guid>,IdentityUserRole<Guid>,
          IdentityUserLogin<Guid>,
          IdentityRoleClaim<Guid>,
          IdentityUserToken<Guid>>(options)
    {
        #region dbset in database
        public DbSet<Category> categories { get; set; }
         public DbSet<Product> products { get; set; }
         public DbSet<Order> orders { get; set; }
         public DbSet<OrderItem> orderItems { get; set; }
         public DbSet<ProductImage> productImages { get; set; }
         public DbSet<Company> companies { get; set; }
         public DbSet<Message> messages { get; set; }
         public DbSet<Payment> payments { get; set; }
         public DbSet<ProductOption> productOptions { get; set; }
         public DbSet<ProductOptionValue> productOptionValues { get; set; }
         public DbSet<ProductPriceTier> productPriceTiers { get; set; }
         public DbSet<ProductVariant> productVariants { get; set; }
         public DbSet<ProductVariantOptionValue> productVariantValues { get; set; }
         public DbSet<RfqQuotation> rfqQuotations { get; set; }
         public DbSet<ProductReview> productReviews { get; set; }
         public DbSet<RfqRequest> rfqRequests { get; set; }
         public DbSet<Role> roles { get; set; }
         public DbSet<Shipping> shippings { get; set; }
         public DbSet<User> users { get; set; }
        public DbSet<Conversation> conversations { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Address> addresses { get; set; }
        public DbSet<ContactInfo> contactInfos { get; set; }   
        public DbSet<ShippingEvent> shippingEvents { get; set; }
        public DbSet<OrderStatusHistory> orderStatusHistories {get; set; }



        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
