using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrustructure.Repository
{
    public class ProductVariantsRepos : GenericRepositoryAsync<ProductVariant>, IProductVariantsRepos
    {
        DbSet<ProductVariant> productVariants;
        public ProductVariantsRepos(AppDBContext context) : base(context)
        {
            productVariants = context.Set<ProductVariant>();
        }
    }
}
