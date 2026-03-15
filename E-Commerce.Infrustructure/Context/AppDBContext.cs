using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace E_Commerce.Infrustructure.Context
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {

        }
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
         public DbSet<ProductVariantValue> productVariantValues { get; set; }
         public DbSet<Quotation> quotations { get; set; }
         public DbSet<Review> reviews { get; set; }
         public DbSet<Rfq> rfqs { get; set; }
         public DbSet<Role> roles { get; set; }
         public DbSet<Shipping> shippings { get; set; }
         public DbSet<User> users { get; set; }

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
