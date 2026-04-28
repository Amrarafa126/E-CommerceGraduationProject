
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InfrustructureBases;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface ICompanyRepos : IGenericRepositoryAsync<Company>
    {
        Task<Company?> GetWithDetailsAsync(Guid companyId, CancellationToken ct = default);
        Task<Company?> GetByOwnerAsync(Guid ownerUserId, CancellationToken ct = default);
        Task<(IEnumerable<Company> Items, int Total)> GetPagedAsync(
            int page, int pageSize, string? search = null, CancellationToken ct = default);
    }
}
