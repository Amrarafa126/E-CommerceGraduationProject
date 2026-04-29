
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;


namespace E_Commerce.Core.Features.Payment.Queries.Handlers
{
    public class PaymentHandlersQueries(IUnitOfWork uow, ICurrentUserService cu)
    : IRequestHandler<GetPaymentByOrderQuery, ApiResponse<PaymentDto>>,
      IRequestHandler<GetWalletQuery, ApiResponse<WalletDto>>,
        IRequestHandler<GetPayoutsQuery, ApiResponse<PaginatedResult<PayoutDto>>>
    {
        public async Task<ApiResponse<PaymentDto>> Handle(GetPaymentByOrderQuery req, CancellationToken ct)
        {
            var payment = await uow.payment.GetByOrderIdAsync(req.OrderId, ct)
                ?? throw new NotFoundException("Payment for order", req.OrderId);

            return ApiResponse<PaymentDto>.Ok(new PaymentDto(
                payment.Id, payment.OrderId, payment.Amount, payment.Currency,
                payment.Status.ToString(), payment.Method.ToString(),
                payment.GatewayTransactionId, payment.CardLast4, payment.CardBrand,
                payment.FailureReason, payment.AmountRefunded,
                payment.PaidAt, payment.RefundedAt, payment.CreatedAt));
        }

        public async Task<ApiResponse<WalletDto>> Handle(GetWalletQuery req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();
            // Role + OwnedCompanyId from JWT — no extra DB call
            if (cu.Role != "Seller" || cu.OwnedCompanyId == null)
                throw new ForbiddenException("Only Sellers have wallets.");
            var wallet = await uow.Wallets.GetByCompanyAsync(cu.OwnedCompanyId.Value, ct)
                ?? throw new NotFoundException("Wallet for company", cu.OwnedCompanyId.Value);

            return ApiResponse<WalletDto>.Ok(new WalletDto(
                wallet.CompanyId, wallet.PendingBalance,
                wallet.AvailableBalance, wallet.TotalEarned, wallet.Currency));
        }

        public async Task<ApiResponse<PaginatedResult<PayoutDto>>> Handle(GetPayoutsQuery req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();
            // Role + OwnedCompanyId from JWT — no extra DB call
            if (cu.Role != "Seller" || cu.OwnedCompanyId == null)
                throw new ForbiddenException("Only Sellers can view payouts.");

            var (items, total) = await uow.payout.GetByCompanyPagedAsync(
                cu.OwnedCompanyId.Value, req.Page, req.PageSize, ct);

            var dtos = items.Select(p => new PayoutDto(
                p.Id, p.CompanyId, p.Amount, p.Currency, p.Status.ToString(),
                p.ExternalReference, p.BankAccountLast4, p.FailureReason,
                p.ProcessedAt, p.CreatedAt)).ToList();

            return ApiResponse<PaginatedResult<PayoutDto>>.Ok(
                PaginatedResult<PayoutDto>.Success(dtos, total, req.Page, req.PageSize));
        }
    }
}
