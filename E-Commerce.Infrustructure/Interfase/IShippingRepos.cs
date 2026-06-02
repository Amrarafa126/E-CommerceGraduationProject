using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InfrustructureBases;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface IShippingRepos : IGenericRepositoryAsync<Shipping>
    {
        Task<Shipping?> GetByOrderSubOrderIdAsync(Guid orderSubOrderId, CancellationToken ct = default);
        Task<Shipping?> GetWithEventsAsync(Guid shipmentId, CancellationToken ct = default);
    }
}
