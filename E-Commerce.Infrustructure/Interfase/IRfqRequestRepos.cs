using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InfrustructureBases;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface IRfqRequestRepos : IGenericRepositoryAsync<RfqRequest>   
    {

        Task<RfqRequest?> GetWithQuotesAsync(Guid rfqId, CancellationToken ct = default);
        Task<(IEnumerable<RfqRequest> Items, int Total)> GetByBuyerPagedAsync(
            Guid buyerId, int page, int pageSize, CancellationToken ct = default);
        Task<(IEnumerable<RfqRequest> Items, int Total)> GetBySellerPagedAsync(
            Guid companyId, int page, int pageSize, CancellationToken ct = default);
    }
}
