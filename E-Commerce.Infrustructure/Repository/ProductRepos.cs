
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

        public Task<IEnumerable<Product>> GetByCompanyAsync(Guid companyId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? categoryId = null, Guid? companyId = null, string? search = null, decimal? minPrice = null, decimal? maxPrice = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> GetProductListAsync()
        {
            var Product = await products.Include(x =>x.Images).ToListAsync();
            return Product;
        }

        public Task<Product?> GetWithFullDetailsAsync(Guid productId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
