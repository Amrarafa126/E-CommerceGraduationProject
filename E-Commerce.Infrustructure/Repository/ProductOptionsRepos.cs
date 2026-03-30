using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrustructure.Repository
{
    public class ProductOptionsRepos : GenericRepositoryAsync<ProductOption>, IProductOptionsRepos
    {
        DbSet<ProductOption> ProductOptions;

        public ProductOptionsRepos(AppDBContext dbContext) : base(dbContext)
        {
            ProductOptions = dbContext.Set<ProductOption>();
        }
    }
}
