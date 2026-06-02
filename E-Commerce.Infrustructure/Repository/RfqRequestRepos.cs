using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrustructure.Repository
{
    public class RfqRequestRepos(AppDBContext Db) : GenericRepositoryAsync<RfqRequest>(Db), IRfqRequestRepos
    {
        public Task<RfqRequest?> GetWithQuotesAsync(Guid id, CancellationToken ct = default)
            => Db.rfqRequests
                .Include(r => r.Buyer)
                .Include(r => r.SellerCompany)
                .Include(r => r.Category)
                .Include(r => r.Product)
                .Include(r => r.Quotes.OrderByDescending(q => q.CreatedAt))
                    .ThenInclude(q => q.SellerCompany)
                .FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task<(IEnumerable<RfqRequest> Items, int Total)> GetByBuyerPagedAsync(
            Guid buyerId, int page, int pageSize, CancellationToken ct = default)
        {
            var q = Db.rfqRequests.AsNoTracking()
                .Include(r => r.SellerCompany)
                .Include(r => r.Category)
                .Include(r => r.Quotes)
                .Where(r => r.BuyerId == buyerId && !r.IsDeleted);
            var total = await q.CountAsync(ct);
            var items = await q.OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }

        public async Task<(IEnumerable<RfqRequest> Items, int Total)> GetBySellerPagedAsync(
            Guid companyId, int page, int pageSize, int? status = null, CancellationToken ct = default)
        {
            var q = Db.rfqRequests.AsNoTracking()
                .Include(r => r.Buyer)
                .Include(r => r.Category)
                .Include(r => r.Quotes)
                .Where(r => r.SellerCompanyId == companyId && !r.IsDeleted);

            if (status.HasValue)
                q = q.Where(r => (int)r.Status == status.Value);

            var total = await q.CountAsync(ct);
            var items = await q.OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }

        public async Task<(IEnumerable<RfqRequest> Items, int Total)> GetMarketplaceAsync(
            int page, int pageSize,
            string? search = null,
            Guid? categoryId = null,
            string? country = null,
            int? status = null,
            CancellationToken ct = default)
        {
            var q = Db.rfqRequests.AsNoTracking()
                .Include(r => r.Buyer)
                .Include(r => r.Category)
                .Where(r => !r.IsDeleted && r.IsPublic);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                q = q.Where(r =>
                    (r.Title != null && r.Title.ToLower().Contains(term)) ||
                    (r.Description != null && r.Description.ToLower().Contains(term)));
            }

            if (categoryId.HasValue)
                q = q.Where(r => r.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(country))
                q = q.Where(r => r.ShippingCountry != null && r.ShippingCountry.ToLower() == country.ToLower());

            if (status.HasValue)
                q = q.Where(r => (int)r.Status == status.Value);
            else
                q = q.Where(r => r.Status == RfqStatus.Pending || r.Status == RfqStatus.Quoted);

            var total = await q.CountAsync(ct);
            var items = await q.OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }

        public Task<RfqRequest?> GetMarketplaceByIdAsync(Guid id, CancellationToken ct = default)
            => Db.rfqRequests.AsNoTracking()
                .Include(r => r.Buyer)
                .Include(r => r.SellerCompany)
                .Include(r => r.Category)
                .Include(r => r.Product)
                .Include(r => r.Quotes.OrderByDescending(q => q.CreatedAt))
                    .ThenInclude(q => q.SellerCompany)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted && r.IsPublic, ct);
    }
}
