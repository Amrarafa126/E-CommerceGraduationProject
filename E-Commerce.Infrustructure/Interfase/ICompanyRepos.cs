
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InfrustructureBases;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface ICompanyRepos : IGenericRepositoryAsync<Company>
    {
        Task<Company?> GetWithProductsAsync(Guid companyId, CancellationToken ct = default);
        Task<Company?> GetWithMembersAsync(Guid companyId, CancellationToken ct = default);
        Task<(IEnumerable<Company> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string? search = null, CancellationToken ct = default);
        public Task<List<Company>> GetCompanyListAsync();
    }
}
