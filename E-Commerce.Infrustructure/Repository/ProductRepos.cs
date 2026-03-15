
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrustructure.Repository
{
    public class ProductRepos :GenericRepositoryAsync<Product>, IProductRepos
    {
        DbSet<Product> products;
        public ProductRepos(AppDBContext dbContext) : base(dbContext)
        {
            products = dbContext.Set<Product>();
        }
    }
}
