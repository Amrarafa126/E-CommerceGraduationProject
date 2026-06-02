using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InfrustructureBases;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface IPaymentRepos : IGenericRepositoryAsync<Payment>
    {
        Task<Payment?> GetByOrderSubOrderIdAsync(Guid orderSubOrderId, CancellationToken ct = default);
        Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken ct = default);
        Task<Payment?> GetByPaymobOrderIdAsync(long paymobOrderId, CancellationToken ct = default);
    }
}
