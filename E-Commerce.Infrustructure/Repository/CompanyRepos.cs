using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrustructure.Repository
{
    public class CompanyRepos(AppDBContext Db) : GenericRepositoryAsync<Company>(Db), ICompanyRepos
    {
    
        public Task<Company?> GetWithDetailsAsync(Guid id, CancellationToken ct = default)
          => Db.companies
              .Include(c => c.Owner)
              .Include(c => c.Wallet)
              .Include(c => c.Employees)
              .FirstOrDefaultAsync(c => c.Id == id, ct);

        public Task<Company?> GetByOwnerAsync(Guid ownerUserId, CancellationToken ct = default)
            => Db.companies.Include(c => c.Wallet)
                .FirstOrDefaultAsync(c => c.OwnerUserId == ownerUserId, ct);
        public async Task<(IEnumerable<Company> Items, int Total)> GetPagedAsync(
            int page, int pageSize, string? search = null, CancellationToken ct = default)
        {
            var q = Db.companies.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(c => c.CompanyName.Contains(search) || c.Description.Contains(search));
            var total = await q.CountAsync(ct);
            var items = await q.OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }
    }
}
