using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Repository
{
    public class PaymentRepos(AppDBContext Db) : GenericRepositoryAsync<Payment>(Db) , IPaymentRepos
    {
        public Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
       => Db.payments.FirstOrDefaultAsync(p => p.OrderId == orderId, ct);
        public Task<Payment?> GetByTransactionIdAsync(string txnId, CancellationToken ct = default)
            => Db.payments.FirstOrDefaultAsync(p => p.GatewayTransactionId == txnId, ct);
    }
}
