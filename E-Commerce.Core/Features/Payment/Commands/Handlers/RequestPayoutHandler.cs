using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Payment.Commands.Handlers
{
    public class RequestPayoutHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<RequestPayoutCommand, ApiResponse<PayoutDto>>
    {
        public async Task<ApiResponse<PayoutDto>> Handle(RequestPayoutCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            if (cu.Role != "Seller" || cu.OwnedCompanyId == null)
                throw new ForbiddenException("يمكن للبائعين أصحاب الشركات فقط طلب السحب.");

            var wallet = await uow.Wallets.GetByCompanyAsync(cu.OwnedCompanyId.Value, ct)
                ?? throw new BusinessException("لم يتم العثور على محفظة الشركة.");

            if (req.Amount > wallet.AvailableBalance)
                throw new BusinessException(
                    $"Insufficient available balance. المتاح: {wallet.AvailableBalance:F2}");

            await uow.BeginTransactionAsync(ct);
            try
            {
                wallet.DebitAvailable(req.Amount);

                var payout = Payout.Create(cu.OwnedCompanyId.Value, req.Amount,
                    wallet.Currency, req.BankAccountLast4);

                await uow.payout.AddAsync(payout, ct);
                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                var dto = mapper.Map<PayoutDto>(payout);
                return ApiResponse<PayoutDto>.Created(dto);
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
    }
}
