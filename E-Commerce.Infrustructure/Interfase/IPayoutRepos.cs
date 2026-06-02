using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InfrustructureBases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface IPayoutRepos : IGenericRepositoryAsync<Payout>
    {
        Task<(IEnumerable<Payout> Items, int Total)> GetByCompanyPagedAsync(
            Guid companyId, int page, int pageSize, CancellationToken ct = default);

        Task<(IEnumerable<Payout> Items, int Total)> GetAllPagedAsync(
            int? status, int page, int pageSize, CancellationToken ct = default);
    }
}
