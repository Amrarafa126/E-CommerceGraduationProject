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
    public class CompanyRepos : GenericRepositoryAsync<Company>, ICompanyRepos
    {
        DbSet<Company> companies;
        public CompanyRepos(AppDBContext dbContext) : base(dbContext)
        {
            companies = dbContext.Set<Company>();
        }

        public Task<List<Company>> GetCompanyListAsync()
        {
            return companies.ToListAsync();

        }

        public Task<(IEnumerable<Company> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Company?> GetWithMembersAsync(Guid companyId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Company?> GetWithProductsAsync(Guid companyId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
