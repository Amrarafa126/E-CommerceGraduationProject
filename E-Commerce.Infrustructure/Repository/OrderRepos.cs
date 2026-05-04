using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;


namespace E_Commerce.Infrustructure.Repository
{
    public class OrderRepos(AppDBContext Db) : GenericRepositoryAsync<Order>(Db), IOrderRepos
    {
        public Task<Order?> GetWithFullDetailsAsync(Guid id, CancellationToken ct = default)
       => Db.orders
           .Include(o => o.Buyer)
           .Include(o => o.SellerCompany)
           .Include(o => o.Payment)
           .Include(o => o.Shipment).ThenInclude(s => s!.Events.OrderBy(e => e.CreatedAt))
           .Include(o => o.Items).ThenInclude(i => i.Product)
           .Include(o => o.Items).ThenInclude(i => i.ProductVariant)
           .Include(o => o.StatusHistory.OrderBy(h => h.CreatedAt))
           .FirstOrDefaultAsync(o => o.Id == id, ct);

        public async Task<(IEnumerable<Order> Items, int Total)> GetByBuyerPagedAsync(
            Guid buyerId, int page, int pageSize, CancellationToken ct = default)
        {
            var q = Db.orders.AsNoTracking()
                .Include(o => o.SellerCompany).Include(o => o.Payment)
                .Include(o => o.Shipment).Include(o => o.Items)
                .Include(o => o.StatusHistory.OrderBy(h => h.CreatedAt))
                .Where(o => o.BuyerId == buyerId);
            var total = await q.CountAsync(ct);
            var items = await q.OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }

        public async Task<(IEnumerable<Order> Items, int Total)> GetBySellerPagedAsync(
            Guid companyId, int page, int pageSize, CancellationToken ct = default)
        {
            var q = Db.orders.AsNoTracking()
                .Include(o => o.Buyer).Include(o => o.Payment)
                .Include(o => o.Shipment).Include(o => o.Items)
                .Include(o => o.StatusHistory.OrderBy(h => h.CreatedAt))
                .Where(o => o.SellerCompanyId == companyId);
            var total = await q.CountAsync(ct);
            var items = await q.OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }

    }
}
