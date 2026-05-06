using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Infrustructure.Repository;
using Microsoft.EntityFrameworkCore.Storage;


namespace E_Commerce.Infrustructure.ImpelmationUnitOfWork
{

    public class UnitOfWork(AppDBContext context) : IUnitOfWork
    {
        private IGenericRepositoryAsync<ProductPriceTier>? _priceTiers;
        private IGenericRepositoryAsync<ProductImage>? _productImages;
        private IDbContextTransaction? _transaction;
        private IUserRepos? _users;
        private ICompanyRepos? _companies;
        private IProductRepos? _products;
        private IOrderRepos? _orders;
        private IOrderItemRepos? _orderItemRepos;
        private IWalletRepos? _wallets;
        private IRfqRequestRepos? _rfqRequest;
        private IRfqQuotationRepos? _rfqQuotation;
        private IConversationRepos? _conversations;
        private IMessageRepos? _messages;
        private IProductReviewRepos? _reviews;
        private IShippingRepos? _shipping;
        private ICategoryRepos? _categories;
        private IPaymentRepos? _payment;
        private IPayoutRepos? _payout;


        //private IDashboardRepository? _dashboard;

        public IGenericRepositoryAsync<ProductPriceTier> PriceTiers
          => _priceTiers ??= new GenericRepositoryAsync<ProductPriceTier>(context);

        public IGenericRepositoryAsync<ProductImage> ProductImages
            => _productImages ??= new GenericRepositoryAsync<ProductImage>(context);
        public IUserRepos Users => _users ??= new UserRepos(context);
        public ICompanyRepos Companies => _companies ??= new CompanyRepos(context);
        public IProductRepos Products => _products ??= new ProductRepos(context);
        public IOrderRepos Orders => _orders ??= new OrderRepos(context);
        public IOrderItemRepos orderItemRepos => _orderItemRepos ??= new OrderItemRepos(context);
        public IWalletRepos Wallets => _wallets ??= new WalletRepos(context);
        public IRfqQuotationRepos RfqQuotation => _rfqQuotation ??= new RfqQuotationRepos(context);
        public IRfqRequestRepos RfqRequest => _rfqRequest ??= new RfqRequestRepos(context);
        public IConversationRepos Conversations => _conversations ??= new ConversationRepos(context);
        public IMessageRepos Messages => _messages ??= new MessageRepos(context);
        public IProductReviewRepos Reviews => _reviews ??= new ProductReviewRepos(context);
        public IShippingRepos shipping => _shipping ??= new ShippingRepos(context);
        public ICategoryRepos Category => _categories ??= new CategoryRepos(context);
        public IPaymentRepos payment => _payment ??= new PaymentRepos(context);
        public IPayoutRepos payout => _payout ??= new PayoutRepos(context);

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await context.SaveChangesAsync(ct);

        public async Task BeginTransactionAsync(CancellationToken ct = default)
            => _transaction = await context.Database.BeginTransactionAsync(ct);

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction == null) return;
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction == null) return;
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        public void Dispose()
        {
            _transaction?.Dispose();
            context.Dispose();
        }
    }
}
