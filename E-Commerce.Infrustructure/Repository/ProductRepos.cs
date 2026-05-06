using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrustructure.Repository
{
    public class ProductRepos(AppDBContext Con) :GenericRepositoryAsync<Product>(Con), IProductRepos
    {
        public async Task<Product?> GetWithFullDetailsAsync(Guid productId, CancellationToken ct = default)
       => await Con.products
           .Include(p => p.Company)
           .Include(p => p.Category)
           .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
           .Include(p => p.ProductOptions.OrderBy(o => o.DisplayOrder))
               .ThenInclude(o => o.Values.OrderBy(v => v.DisplayOrder))
           .Include(p => p.productVariants)
               .ThenInclude(v => v.OptionValues)
                   .ThenInclude(ov => ov.ProductOptionValue)
                       .ThenInclude(pov => pov.ProductOption)
           .Include(p => p.PriceTiers.OrderBy(t => t.MinQuantity))
           .FirstOrDefaultAsync(p => p.Id == productId, ct);

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            Guid? categoryId = null,
            Guid? companyId = null,
            string? search = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            CancellationToken ct = default)
        {
            var query = Context.products
                .AsNoTracking()
                .Include(p => p.Company)
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (companyId.HasValue)
                query = query.Where(p => p.CompanyId == companyId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.Description.Contains(search));

            if (minPrice.HasValue)
                query = query.Where(p => p.BasePrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.BasePrice <= maxPrice.Value);

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }
        public async Task<IEnumerable<Product>> GetByCompanyAsync(Guid companyId, CancellationToken ct = default)
            => await Con.products
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId)
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(ct);
    }
}
