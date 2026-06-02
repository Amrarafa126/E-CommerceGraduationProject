using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Commands.Models;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Payment.Commands.Handlers
{
    public class RefundPaymentHandler(
        IUnitOfWork uow,
        IPaymentGateway gateway,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<RefundPaymentCommand, ApiResponse<PaymentDto>>
    {
        public async Task<ApiResponse<PaymentDto>> Handle(RefundPaymentCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();
            if (cu.Role != "Admin")
                throw new ForbiddenException("يمكن للمسؤولين فقط معالجة الاسترجاع.");

            var payment = await uow.payment.GetByIdAsync(req.PaymentId, ct)
                ?? throw new NotFoundException(nameof(Payment), req.PaymentId);

            if (!payment.IsPaid)
                throw new BusinessException("يمكن استرجاع فقط المدفوعات المسددة.");
            if (req.Amount > payment.RemainingAmount)
                throw new BusinessException($"مبلغ الاسترجاع يتجاوز المتبقي: {payment.RemainingAmount}.");

            await uow.BeginTransactionAsync(ct);
            try
            {
                // Call gateway refund
                if (payment.GatewayTransactionId != null)
                {
                    var refundResult = await gateway.RefundAsync(
                        new GatewayRefundRequest(payment.GatewayTransactionId, req.Amount, req.Reason), ct);

                    if (!refundResult.Success)
                        throw new BusinessException($"فشل استرجاع بوابة الدفع: {refundResult.ErrorMessage}");
                }

                payment.Refund(req.Amount);

                // Find sub-order and debit from seller wallet
                var order = await uow.Orders.GetWithFullDetailsAsync(payment.OrderSubOrderId, ct);
                var subOrder = order?.SubOrders.FirstOrDefault(s => s.Id == payment.OrderSubOrderId);

                if (subOrder != null)
                {
                    var wallet = await uow.Wallets.GetByCompanyAsync(subOrder.SellerCompanyId, ct);
                    if (wallet != null)
                    {
                        if (wallet.AvailableBalance >= req.Amount)
                        {
                            wallet.DebitAvailable(req.Amount);
                        }
                        else if (wallet.PendingBalance >= req.Amount)
                        {
                            wallet.ReleasePending(req.Amount);
                            wallet.DebitAvailable(req.Amount);
                        }
                        else
                        {
                            throw new BusinessException("محفظة البائع لا تحتوي على رصيد كافٍ للاسترجاع.");
                        }
                    }

                    if (payment.Status == PaymentStatus.Refunded)
                        subOrder.MarkRefunded();
                }

                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                var dto = mapper.Map<PaymentDto>(payment);
                return ApiResponse<PaymentDto>.Ok(dto);
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
    }
}
