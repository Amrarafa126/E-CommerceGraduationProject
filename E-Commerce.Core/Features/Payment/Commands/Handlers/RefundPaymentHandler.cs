using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Commands.Models;
using E_Commerce.Data.Status;
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
    public class RefundPaymentHandler(IUnitOfWork uow, IPaymentGateway gateway)
    : IRequestHandler<RefundPaymentCommand, ApiResponse<PaymentDto>>
    {
        public async Task<ApiResponse<PaymentDto>> Handle(RefundPaymentCommand req, CancellationToken ct)
        {
            var payment = await uow.payment.GetByIdAsync(req.PaymentId, ct)
                ?? throw new NotFoundException(nameof(Payment), req.PaymentId);

            if (!payment.IsPaid) throw new BusinessException("Only paid payments can be refunded.");
            if (req.Amount > payment.RemainingAmount)
                throw new BusinessException($"Refund amount exceeds remaining: {payment.RemainingAmount}.");

            await uow.BeginTransactionAsync(ct);
            try
            {
                // Call gateway refund
                if (payment.GatewayTransactionId != null)
                {
                    var refundResult = await gateway.RefundAsync(
                        new GatewayRefundRequest(payment.GatewayTransactionId, req.Amount, req.Reason), ct);

                    if (!refundResult.Success)
                        throw new BusinessException($"Gateway refund failed: {refundResult.ErrorMessage}");
                }

                payment.Refund(req.Amount);
                uow.payment.Update(payment);

                // Debit from seller wallet
                var order = await uow.Orders.GetByIdAsync(payment.OrderId, ct)!;
                if (order != null)
                {
                    var wallet = await uow.Wallets.GetByCompanyAsync(order.SellerCompanyId, ct);
                    if (wallet != null)
                    {
                        // Try to debit from available first, then pending
                        if (wallet.AvailableBalance >= req.Amount)
                            wallet.DebitAvailable(req.Amount);
                        else if (wallet.PendingBalance >= req.Amount)
                            wallet.ReleasePending(req.Amount); // move to available then debit — simplified
                        uow.Wallets.Update(wallet);
                    }

                    if (payment.Status == PaymentStatus.Refunded)
                        order.MarkRefunded();
                    uow.Orders.Update(order);
                }

                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                return ApiResponse<PaymentDto>.Ok(new PaymentDto(
                    payment.Id, payment.OrderId, payment.Amount, payment.Currency,
                    payment.Status.ToString(), payment.Method.ToString(),
                    payment.GatewayTransactionId, payment.CardLast4, payment.CardBrand,
                    payment.FailureReason, payment.AmountRefunded,
                    payment.PaidAt, payment.RefundedAt, payment.CreatedAt));
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
    }

}
