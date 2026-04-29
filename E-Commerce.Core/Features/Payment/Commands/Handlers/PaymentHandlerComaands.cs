using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Commands.Models;
using E_Commerce.Data.Entity;
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
    internal class PaymentHandlerComaands(
    IUnitOfWork uow,
    IPaymentGateway gateway,
    ICurrentUserService cu)
    : IRequestHandler<InitiatePaymentCommand, ApiResponse<PaymentDto>>
    {
        public async Task<ApiResponse<PaymentDto>> Handle(InitiatePaymentCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var order = await uow.Orders.GetWithFullDetailsAsync(req.OrderId, ct)
                ?? throw new NotFoundException(nameof(Order), req.OrderId);

            if (order.BuyerId != cu.UserId.Value)
                throw new ForbiddenException("You can only pay for your own orders.");

            if (order.Status != OrderStatus.Pending)
                throw new BusinessException($"Order is not payable (status: {order.Status}).");

            if (order.PaymentId.HasValue)
                throw new ConflictException("A payment already exists for this order.");

            if (!Enum.TryParse<PaymentMethod>(req.PaymentMethod, true, out var method))
                throw new ValidationException("Invalid payment method.");

            await uow.BeginTransactionAsync(ct);
            try
            {
                // 1. Create payment record
                var payment = Data.Entity.Payment.Create(order.Id, order.TotalAmount, method, req.Currency);
                await uow.payment.AddAsync(payment, ct);
                await uow.SaveChangesAsync(ct);

                // 2. Call gateway (CashOnDelivery skips gateway)
                if (method != PaymentMethod.CashOnDelivery)
                {
                    var chargeReq = new GatewayChargeRequest(
                        Amount: order.TotalAmount,
                        Currency: req.Currency,
                        PaymentMethod: req.PaymentMethod.ToLower(),
                        CardToken: req.CardToken,
                        CustomerId: cu.UserId.Value.ToString(),
                        OrderReference: order.OrderNumber,
                        Description: $"Payment for order {order.OrderNumber}",
                        Metadata: new Dictionary<string, string>
                        {
                            ["order_id"] = order.Id.ToString(),
                            ["buyer_id"] = cu.UserId.Value.ToString()
                        });

                    var result = await gateway.ChargeAsync(chargeReq, ct);

                    if (result.Success)
                    {
                        payment.MarkPaid(result.TransactionId!, result.GatewayReference,
                            result.CardLast4, result.CardBrand);
                    }
                    else
                    {
                        payment.MarkFailed(result.ErrorMessage ?? "Gateway error.", result.RawResponse);
                        uow.payment.Update(payment);
                        await uow.SaveChangesAsync(ct);
                        await uow.CommitTransactionAsync(ct);
                        return ApiResponse<PaymentDto>.Fail(
                            $"Payment failed: {result.ErrorMessage}", 402);
                    }
                }
                else
                {
                    // COD: payment confirmed on delivery, but order can proceed
                    payment.MarkPaid("COD-" + Guid.NewGuid().ToString("N")[..12]);
                }

                // 3. Link payment → order, update order status
                order.LinkPayment(payment.Id);
                order.MarkPaid();

                // 4. Credit seller wallet (held as pending until delivery)
                var wallet = await uow.Wallets.GetByCompanyAsync(order.SellerCompanyId, ct)
                    ?? throw new BusinessException("Seller wallet not found.");
                wallet.CreditPending(order.TotalAmount);

                uow.payment.Update(payment);
                uow.Orders.Update(order);
                uow.Wallets.Update(wallet);
                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                return ApiResponse<PaymentDto>.Created(MapPayment(payment));
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }

        private static PaymentDto MapPayment(Data.Entity.Payment p) =>
            new(p.Id, p.OrderId, p.Amount, p.Currency, p.Status.ToString(),
                p.Method.ToString(), p.GatewayTransactionId,
                p.CardLast4, p.CardBrand, p.FailureReason,
                p.AmountRefunded, p.PaidAt, p.RefundedAt, p.CreatedAt);
    }
}
