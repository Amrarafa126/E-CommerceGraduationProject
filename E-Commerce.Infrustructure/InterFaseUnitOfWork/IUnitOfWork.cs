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
        IProductRepos Products { get; }
        IOrderRepos Orders { get; }
        IOrderItemRepos orderItemRepos { get; }
        IWalletRepos Wallets { get; }
        IProductOptionValuesRepos ProductOptionValues { get; }
            IProductOptionsRepos ProductOptions { get; }

        IProductImageRepos productImage { get; }
        IProductPriceTierRepos productPrice { get; }
        IProductVariantsRepos productVariantsRepos { get; }
        IProductVariantValueRepos productVariantValueRepos { get; }
        IRefreshTokenRepository refreshTokenRepository { get; }
        IRfqRequestRepos RfqRequest { get; }
        IRfqQuotationRepos RfqQuotation { get; }
        IConversationRepos Conversations { get; }
        IMessageRepos Messages { get; }
        IProductReviewRepos Reviews { get; }
        //   IDashboardRepository Dashboard { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync(CancellationToken ct = default);
        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
    }
