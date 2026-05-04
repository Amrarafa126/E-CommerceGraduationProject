using E_Commerce.Infrustructure.Interfase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.InterFaseUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepos Users { get; }
        ICompanyRepos Companies { get; }
        ICategoryRepos Category { get; }
        IProductRepos Products { get; }
        IOrderRepos Orders { get; }
        IOrderItemRepos orderItemRepos { get; }
        IWalletRepos Wallets { get; } 
        IRfqRequestRepos RfqRequest { get; }
        IRfqQuotationRepos RfqQuotation { get; }
        IConversationRepos Conversations { get; }
        IMessageRepos Messages { get; }
        IProductReviewRepos Reviews { get; }
        IShippingRepos shipping {  get; }
        IPaymentRepos payment { get; }
        IPayoutRepos payout { get; }    
        
        //   IDashboardRepository Dashboard { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync(CancellationToken ct = default);
        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
    }
