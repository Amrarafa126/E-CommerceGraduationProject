using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrustructure.Repository
{
    public class ProductImageRepos : GenericRepositoryAsync<ProductImage>, IProductImageRepos
    {
        DbSet<ProductImage> ProductImages;
        public ProductImageRepos(AppDBContext dbContext) : base(dbContext)
        {
            ProductImages = dbContext.Set<ProductImage>();
        }

        public async Task<List<ProductImage>> AddListAsync(List<ProductImage> entity)
        {
            try
            {
               await ProductImages.AddRangeAsync(entity);
                return (entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding product images", ex);
            }

        }
    }
}
