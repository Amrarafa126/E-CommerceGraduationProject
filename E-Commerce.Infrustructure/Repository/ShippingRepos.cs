using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrustructure.Repository
{
    public class ShippingRepos(AppDBContext Db) : GenericRepositoryAsync<Shipping>(Db), IShippingRepos
    {
        public Task<Shipping?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
       => Db.shippings.Include(s => s.Events.OrderBy(e => e.CreatedAt))
           .FirstOrDefaultAsync(s => s.OrderId == orderId, ct);
        public Task<Shipping?> GetWithEventsAsync(Guid id, CancellationToken ct = default)
            => Db.shippings.Include(s => s.Events.OrderBy(e => e.CreatedAt))
                .FirstOrDefaultAsync(s => s.Id == id, ct);
    }
}
