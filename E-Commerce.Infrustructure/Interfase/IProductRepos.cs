using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InfrustructureBases;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface IProductRepos : IGenericRepositoryAsync<Product>
    {
        Task<Product?> GetWithFullDetailsAsync(Guid productId, CancellationToken ct = default);
        Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            Guid? categoryId = null,
            Guid? companyId = null,
            string? search = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            CancellationToken ct = default);
        Task<IEnumerable<Product>> GetByCompanyAsync(Guid companyId, CancellationToken ct = default);
        public Task<List<Product>> GetProductListAsync();

    }
}
