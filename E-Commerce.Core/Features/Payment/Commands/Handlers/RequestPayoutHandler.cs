using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Payment.Commands.Handlers
{

    public class RequestPayoutHandler(IUnitOfWork uow, ICurrentUserService cu)
        : IRequestHandler<RequestPayoutCommand, ApiResponse<PayoutDto>>
    {
        public async Task<ApiResponse<PayoutDto>> Handle(RequestPayoutCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            // Role + OwnedCompanyId from JWT — no extra DB call
            if (cu.Role != "Seller" || cu.OwnedCompanyId == null)
                throw new ForbiddenException("Only Sellers with a company can request payouts.");

            var wallet = await uow.Wallets.GetByCompanyAsync(cu.OwnedCompanyId!.Value, ct)
                ?? throw new BusinessException("Company wallet not found.");

            if (req.Amount > wallet.AvailableBalance)
                throw new BusinessException(
                    $"Insufficient available balance. Available: {wallet.AvailableBalance:F2}");

            await uow.BeginTransactionAsync(ct);
            try
            {
                wallet.DebitAvailable(req.Amount);
                uow.Wallets.Update(wallet);

                var payout = Payout.Create(cu.OwnedCompanyId!.Value, req.Amount,
                    wallet.Currency, req.BankAccountLast4);

                // Simulate async payout processing
                payout.MarkProcessing("PAYOUT-" + Guid.NewGuid().ToString("N")[..16].ToUpper());
                payout.MarkCompleted(); // In real life: webhook from payment provider

                await uow.payout.AddAsync(payout, ct);
                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                return ApiResponse<PayoutDto>.Created(new PayoutDto(
                    payout.Id, payout.CompanyId, payout.Amount, payout.Currency,
                    payout.Status.ToString(), payout.ExternalReference,
                    payout.BankAccountLast4, payout.FailureReason,
                    payout.ProcessedAt, payout.CreatedAt));
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
    }
}
