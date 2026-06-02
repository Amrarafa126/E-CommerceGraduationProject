using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Payment.Commands.Handlers
{
    public class InitiatePaymentHandler(
        IUnitOfWork uow,
        IPaymentGateway gateway,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<InitiatePaymentCommand, ApiResponse<PaymentDto>>
    {
        public async Task<ApiResponse<PaymentDto>> Handle(InitiatePaymentCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var order = await uow.Orders.GetWithFullDetailsAsync(req.OrderId, ct)
                ?? throw new NotFoundException(nameof(Order), req.OrderId);

            if (order.BuyerId != cu.UserId.Value)
                throw new ForbiddenException("يمكنك الدفع فقط مقابل طلباتك.");

            // For backward compatibility, use the first (and typically only) sub-order
            var subOrder = order.SubOrders.FirstOrDefault()
                ?? throw new BusinessException("الطلب لا يحتوي على طلبات فرعية.");

            if (subOrder.Status != OrderStatus.Pending)
                throw new BusinessException($"الطلب غير قابل للدفع (الحالة: {subOrder.Status}).");

            if (subOrder.PaymentId.HasValue)
                throw new ConflictException("يوجد دفع مسجل لهذا الطلب بالفعل.");

            var method = (PaymentMethod)(req.PaymentMethod + 1);
            if (!Enum.IsDefined(typeof(PaymentMethod), method))
                throw new ValidationException("طريقة الدفع غير صالحة.");

            await uow.BeginTransactionAsync(ct);
            try
            {
                var payment = Data.Entity.Payment.Create(subOrder.Id, subOrder.TotalAmount, method, req.Currency, req.IdempotencyKey);
                await uow.payment.AddAsync(payment, ct);

                bool gatewayFailed = false;
                string? failureMessage = null;

                // For Card/Wallet: create Paymob order and get payment key
                if (method is PaymentMethod.Card or PaymentMethod.Wallet)
                {
                    var chargeReq = new GatewayChargeRequest(
                        Amount: subOrder.TotalAmount,
                        Currency: req.Currency,
                        PaymentMethod: method.ToString().ToLower(),
                        CardToken: req.CardToken,
                        CustomerId: cu.UserId.Value.ToString(),
                        OrderReference: order.OrderNumber,
                        Description: $"دفع الطلب {order.OrderNumber}",
                        Metadata: new Dictionary<string, string>
                        {
                            ["order_id"] = order.Id.ToString(),
                            ["sub_order_id"] = subOrder.Id.ToString(),
                            ["buyer_id"] = cu.UserId.Value.ToString()
                        });

                    var result = await gateway.ChargeAsync(chargeReq, ct);

                    if (result.Success)
                    {
                        if (!string.IsNullOrEmpty(result.TransactionId))
                        {
                            payment.SetPaymobDetails(
                                long.Parse(result.TransactionId),
                                result.GatewayReference ?? string.Empty);
                        }
                    }
                    else
                    {
                        payment.MarkFailed(result.ErrorMessage ?? "Gateway error.", result.RawResponse);
                        gatewayFailed = true;
                        failureMessage = result.ErrorMessage;
                    }
                }
                else if (method == PaymentMethod.CashOnDelivery)
                {
                    // COD: payment stays Pending until delivery
                    // No gateway call needed
                }
                else if (method == PaymentMethod.BankTransfer)
                {
                    // Bank transfer: stays Pending until admin confirms
                    // No gateway call needed
                }

                // Link payment to sub-order (but do NOT mark as paid or credit wallet yet)
                // Payment confirmation happens via:
                // - Card/Wallet: Paymob webhook
                // - COD: delivery confirmation (UpdateShipmentStatusHandler)
                // - BankTransfer: admin manual confirmation
                if (!gatewayFailed && method != PaymentMethod.BankTransfer)
                {
                    subOrder.LinkPayment(payment.Id);
                }

                order.RecalculateTotals();

                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                if (gatewayFailed)
                    return ApiResponse<PaymentDto>.Fail($"فشل الدفع: {failureMessage}", 402);

                var dto = mapper.Map<PaymentDto>(payment);
                return ApiResponse<PaymentDto>.Created(dto);
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
    }
}
