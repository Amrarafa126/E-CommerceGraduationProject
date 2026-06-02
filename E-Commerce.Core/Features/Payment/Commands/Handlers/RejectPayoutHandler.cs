using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Commands.Models;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using MediatR;

namespace E_Commerce.Core.Features.Payment.Commands.Handlers
{
    public class RejectPayoutHandler(
        IUnitOfWork uow,
        IMapper mapper)
        : IRequestHandler<RejectPayoutCommand, ApiResponse<PayoutDto>>
    {
        public async Task<ApiResponse<PayoutDto>> Handle(RejectPayoutCommand req, CancellationToken ct)
        {
            var payout = await uow.payout.GetByIdAsync(req.PayoutId, ct)
                ?? throw new NotFoundException("طلب السحب غير موجود.");

            if (payout.Status != PayoutStatus.Pending)
                throw new BusinessException("يمكن رفض فقط طلبات السحب المعلقة.");

            await uow.BeginTransactionAsync(ct);
            try
            {
                var wallet = await uow.Wallets.GetByCompanyAsync(payout.CompanyId, ct)
                    ?? throw new BusinessException("لم يتم العثور على محفظة الشركة.");

                wallet.CreditAvailable(payout.Amount);
                payout.MarkFailed(req.Reason);

                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                var dto = mapper.Map<PayoutDto>(payout);
                return ApiResponse<PayoutDto>.Ok(dto);
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
    }
}
