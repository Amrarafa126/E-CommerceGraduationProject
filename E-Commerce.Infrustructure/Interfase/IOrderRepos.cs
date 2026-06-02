using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InfrustructureBases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface IOrderRepos : IGenericRepositoryAsync<Order>
    {
        Task<Order?> GetWithFullDetailsAsync(Guid orderId, CancellationToken ct = default);
        Task<(IEnumerable<Order> Items, int Total)> GetByBuyerPagedAsync(
            Guid buyerId, int page, int pageSize, CancellationToken ct = default);
        Task<(IEnumerable<Order> Items, int Total)> GetBySellerPagedAsync(
            Guid companyId, int page, int pageSize, CancellationToken ct = default);
        Task<(IEnumerable<OrderSubOrder> Items, int Total)> GetSubOrdersBySellerPagedAsync(
            Guid companyId, int page, int pageSize, CancellationToken ct = default);
    }
}
