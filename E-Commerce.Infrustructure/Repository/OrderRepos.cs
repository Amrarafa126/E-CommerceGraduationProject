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
           .Include(o => o.ShippingAddress)
           .Include(o => o.Buyer)
           .Include(o => o.SubOrders)
               .ThenInclude(s => s.SellerCompany)
           .Include(o => o.SubOrders)
               .ThenInclude(s => s.Payment)
           .Include(o => o.SubOrders)
               .ThenInclude(s => s.Shipment)
               .ThenInclude(sh => sh!.Events.OrderBy(e => e.CreatedAt))
           .Include(o => o.SubOrders)
               .ThenInclude(s => s.Items)
               .ThenInclude(i => i.Product)
           .Include(o => o.SubOrders)
               .ThenInclude(s => s.Items)
               .ThenInclude(i => i.ProductVariant)
           .Include(o => o.SubOrders)
               .ThenInclude(s => s.StatusHistory.OrderBy(h => h.CreatedAt))
           .FirstOrDefaultAsync(o => o.Id == id, ct);

        public async Task<(IEnumerable<Order> Items, int Total)> GetByBuyerPagedAsync(
            Guid buyerId, int page, int pageSize, CancellationToken ct = default)
        {
            var q = Db.orders.AsNoTracking()
                .Include(o => o.ShippingAddress)
                .Include(o => o.SubOrders)
                    .ThenInclude(s => s.SellerCompany)
                .Include(o => o.SubOrders)
                    .ThenInclude(s => s.Payment)
                .Include(o => o.SubOrders)
                    .ThenInclude(s => s.Items)
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
                .Include(o => o.ShippingAddress)
                .Include(o => o.Buyer)
                .Include(o => o.SubOrders)
                    .ThenInclude(s => s.Payment)
                .Include(o => o.SubOrders)
                    .ThenInclude(s => s.Items)
                .Where(o => o.SubOrders.Any(s => s.SellerCompanyId == companyId));
            var total = await q.CountAsync(ct);
            var items = await q.OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }

        public async Task<(IEnumerable<OrderSubOrder> Items, int Total)> GetSubOrdersBySellerPagedAsync(
            Guid companyId, int page, int pageSize, CancellationToken ct = default)
        {
            var q = Db.orderSubOrders.AsNoTracking()
                .Include(s => s.Order)
                    .ThenInclude(o => o.Buyer)
                .Include(s => s.SellerCompany)
                .Include(s => s.Payment)
                .Include(s => s.Shipment)
                .Include(s => s.Items)
                .Include(s => s.StatusHistory.OrderBy(h => h.CreatedAt))
                .Where(s => s.SellerCompanyId == companyId);
            var total = await q.CountAsync(ct);
            var items = await q.OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }
    }
}
