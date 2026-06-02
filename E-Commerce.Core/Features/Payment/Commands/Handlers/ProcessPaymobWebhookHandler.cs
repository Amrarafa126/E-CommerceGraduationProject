using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Payment;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace E_Commerce.Core.Features.Payment.Commands.Handlers
{
    public class ProcessPaymobWebhookHandler(
        IUnitOfWork uow,
        IPaymobClient paymobClient,
        IOptions<PaymobOptions> paymobOptions,
        ILogger<ProcessPaymobWebhookHandler> logger)
        : IRequestHandler<ProcessPaymobWebhookCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<object>> Handle(ProcessPaymobWebhookCommand req, CancellationToken ct)
        {
            // 1. Find payment by PaymobOrderId
            var payment = await uow.payment.GetByPaymobOrderIdAsync(req.OrderId, ct);

            if (payment == null)
            {
                logger.LogWarning("Paymob webhook: payment not found for order {PaymobOrderId}", req.OrderId);
                return ApiResponse<object>.Fail("الدفع غير موجود.", 404);
            }

            if (payment.Status != PaymentStatus.Pending)
            {
                logger.LogInformation("Paymob webhook: payment {PaymentId} already processed with status {Status}",
                    payment.Id, payment.Status);
                return ApiResponse<object>.Ok("تم معالجة الدفع بالفعل.");
            }

            // 2. Validate amount (convert cents to decimal for comparison)
            var receivedAmount = req.AmountCents / 100m;
            if (receivedAmount != payment.Amount)
            {
                logger.LogWarning("Paymob webhook: amount mismatch. Expected {Expected}, received {Received}",
                    payment.Amount, receivedAmount);
                return ApiResponse<object>.Fail("المبلغ غير متطابق.", 400);
            }

            // 3. Find sub-order and parent order
            var subOrder = await uow.OrderSubOrders.GetByIdAsync(payment.OrderSubOrderId, ct)
                ?? throw new NotFoundException("الطلب الفرعي", payment.OrderSubOrderId);

            var order = await uow.Orders.GetWithFullDetailsAsync(subOrder.OrderId, ct)
                ?? throw new NotFoundException(nameof(Order), subOrder.OrderId);

            subOrder = order.SubOrders.FirstOrDefault(s => s.Id == payment.OrderSubOrderId)
                ?? throw new NotFoundException("الطلب الفرعي", payment.OrderSubOrderId);

            await uow.BeginTransactionAsync(ct);
            try
            {
                if (req.Success)
                {
                    payment.MarkPaid(
                        req.TransactionId ?? $"paymob_{req.OrderId}",
                        req.TransactionId,
                        null, null);

                    subOrder.LinkPayment(payment.Id);
                    subOrder.MarkPaid();

                    var wallet = await uow.Wallets.GetByCompanyAsync(subOrder.SellerCompanyId, ct)
                        ?? throw new BusinessException("لم يتم العثور على محفظة البائع.");
                    wallet.CreditPending(subOrder.TotalAmount);

                    order.RecalculateTotals();

                    logger.LogInformation("Paymob webhook: payment {PaymentId} marked as paid.", payment.Id);
                }
                else
                {
                    payment.MarkFailed("Payment failed via Paymob webhook.");
                    logger.LogInformation("Paymob webhook: payment {PaymentId} marked as failed.", payment.Id);
                }

                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                return ApiResponse<object>.Ok("تم معالجة Webhook بنجاح.");
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
    }
}
