using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Commands.Models;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using MediatR;

namespace E_Commerce.Core.Features.Payment.Commands.Handlers
{
    public class ApprovePayoutHandler(
        IUnitOfWork uow,
        IMapper mapper)
        : IRequestHandler<ApprovePayoutCommand, ApiResponse<PayoutDto>>
    {
        public async Task<ApiResponse<PayoutDto>> Handle(ApprovePayoutCommand req, CancellationToken ct)
        {
            var payout = await uow.payout.GetByIdAsync(req.PayoutId, ct)
                ?? throw new NotFoundException("طلب السحب غير موجود.");

            if (payout.Status != PayoutStatus.Pending)
                throw new BusinessException("يمكن الموافقة فقط على طلبات السحب المعلقة.");

            await uow.BeginTransactionAsync(ct);
            try
            {
                var externalRef = req.ExternalReference
                    ?? ("PAYOUT-" + Guid.NewGuid().ToString("N")[..16].ToUpper());
                payout.MarkProcessing(externalRef);
                payout.MarkCompleted();

                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                var dto = mapper.Map<PayoutDto>(payout);
                return ApiResponse<PayoutDto>.Ok(dto);
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
    }
}
