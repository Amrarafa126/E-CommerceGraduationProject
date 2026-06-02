using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrustructure.Repository
{
    public class PaymentRepos(AppDBContext Db) : GenericRepositoryAsync<Payment>(Db), IPaymentRepos
    {
        public Task<Payment?> GetByOrderSubOrderIdAsync(Guid orderSubOrderId, CancellationToken ct = default)
            => Db.payments.FirstOrDefaultAsync(p => p.OrderSubOrderId == orderSubOrderId, ct);
        public Task<Payment?> GetByTransactionIdAsync(string txnId, CancellationToken ct = default)
            => Db.payments.FirstOrDefaultAsync(p => p.GatewayTransactionId == txnId, ct);
        public Task<Payment?> GetByPaymobOrderIdAsync(long paymobOrderId, CancellationToken ct = default)
            => Db.payments.FirstOrDefaultAsync(p => p.PaymobOrderId == paymobOrderId, ct);
    }
}
