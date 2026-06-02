using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;


namespace E_Commerce.Infrustructure.Repository
{
    public class PayoutRepos(AppDBContext Db) : GenericRepositoryAsync<Payout>(Db) , IPayoutRepos
    {
        public async Task<(IEnumerable<Payout> Items, int Total)> GetByCompanyPagedAsync(
            Guid companyId, int page, int pageSize, CancellationToken ct = default)
        {
            var q = Db.payouts.AsNoTracking()
                .Where(p => p.CompanyId == companyId).OrderByDescending(p => p.CreatedAt);
            var total = await q.CountAsync(ct);
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }

        public async Task<(IEnumerable<Payout> Items, int Total)> GetAllPagedAsync(
            int? status, int page, int pageSize, CancellationToken ct = default)
        {
            var q = Db.payouts.AsNoTracking().AsQueryable();
            if (status.HasValue)
                q = q.Where(p => (int)p.Status == status.Value);
            q = q.OrderByDescending(p => p.CreatedAt);
            var total = await q.CountAsync(ct);
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }
    }
}
