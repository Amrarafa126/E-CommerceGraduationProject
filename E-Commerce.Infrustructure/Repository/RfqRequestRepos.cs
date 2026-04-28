using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Repository
{
    public class RfqRequestRepos(AppDBContext Db) : GenericRepositoryAsync<RfqRequest>(Db), IRfqRequestRepos
    {
       
        public Task<RfqRequest?> GetWithQuotesAsync(Guid id, CancellationToken ct = default)
        => Db.rfqRequests
            .Include(r => r.Buyer).Include(r => r.SellerCompany).Include(r => r.Product)
            .Include(r => r.Quotes.OrderByDescending(q => q.CreatedAt))
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task<(IEnumerable<RfqRequest> Items, int Total)> GetByBuyerPagedAsync(
            Guid buyerId, int page, int pageSize, CancellationToken ct = default)
        {
            var q = Db.rfqRequests.AsNoTracking()
                .Include(r => r.SellerCompany).Include(r => r.Quotes)
                .Where(r => r.BuyerId == buyerId && !r.IsDeleted);
            var total = await q.CountAsync(ct);
            var items = await q.OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }

        public async Task<(IEnumerable<RfqRequest> Items, int Total)> GetBySellerPagedAsync(
            Guid companyId, int page, int pageSize, CancellationToken ct = default)
        {
            var q = Db.rfqRequests.AsNoTracking()
                .Include(r => r.Buyer).Include(r => r.Quotes)
                .Where(r => r.SellerCompanyId == companyId && !r.IsDeleted);
            var total = await q.CountAsync(ct);
            var items = await q.OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }
    }
}
