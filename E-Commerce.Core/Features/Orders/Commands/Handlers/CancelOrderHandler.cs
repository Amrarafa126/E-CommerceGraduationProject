using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Orders.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Orders.Commands.Handlers
{
    public class CancelOrderHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<CancelOrderCommand, ApiResponse<OrderDto>>
    {
        public async Task<ApiResponse<OrderDto>> Handle(CancelOrderCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var order = await uow.Orders.GetWithFullDetailsAsync(req.OrderId, ct)
                ?? throw new NotFoundException(nameof(Order), req.OrderId);

            bool isBuyer = order.BuyerId == cu.UserId.Value;
            bool isSeller = cu.OwnedCompanyId != null && order.SubOrders.Any(s => s.SellerCompanyId == cu.OwnedCompanyId.Value);
            bool isAdmin = cu.Role == "Admin";

            if (!isBuyer && !isSeller && !isAdmin)
                throw new ForbiddenException("غير مصرح لك بإلغاء هذا الطلب.");

            if (isSeller && cu.OwnedCompanyId.HasValue)
            {
                // Seller can only cancel their own sub-order(s)
                var subOrders = order.SubOrders.Where(s => s.SellerCompanyId == cu.OwnedCompanyId.Value).ToList();
                foreach (var sub in subOrders)
                {
                    if (sub.Status is OrderStatus.Shipped
                        or OrderStatus.Delivered
                        or OrderStatus.Completed
                        or OrderStatus.Cancelled)
                        throw new BusinessException("لا يمكن إلغاء طلب فرعي تم شحنه أو توصيله أو اكتماله أو إلغاؤه بالفعل.");

                    await CancelSubOrderAsync(sub, req.Reason, ct);
                }
            }
            else if (isBuyer || isAdmin)
            {
                // Buyer/Admin cancels all sub-orders
                foreach (var sub in order.SubOrders.Where(s => s.Status != OrderStatus.Cancelled
                    && s.Status != OrderStatus.Shipped
                    && s.Status != OrderStatus.Delivered
                    && s.Status != OrderStatus.Completed))
                {
                    await CancelSubOrderAsync(sub, req.Reason, ct);
                }
            }

            order.RecalculateTotals();
            await uow.SaveChangesAsync(ct);

            var dto = mapper.Map<OrderDto>(order);
            return ApiResponse<OrderDto>.Ok(dto);
        }

        private async Task CancelSubOrderAsync(OrderSubOrder sub, string reason, CancellationToken ct)
        {
            // 1. Restore stock for ALL items (variants and non-variants)
            foreach (var item in sub.Items)
            {
                var product = await uow.Products.GetWithFullDetailsAsync(item.ProductId, ct);
                if (product == null) continue;

                if (item.ProductVariantId.HasValue)
                {
                    var variant = product.productVariants.FirstOrDefault(v => v.Id == item.ProductVariantId);
                    variant?.AdjustStock(item.Quantity);
                }
                else
                {
                    // Non-variant: restore product stock
                    product.SetStock(product.StockQuantity + item.Quantity);
                }
            }

            // 2. Reverse wallet credit if payment was made
            if (sub.Status == OrderStatus.Paid && sub.PaymentId.HasValue)
            {
                var payment = await uow.payment.GetByIdAsync(sub.PaymentId.Value, ct);
                if (payment != null && payment.Status == PaymentStatus.Paid)
                {
                    // Refund the payment
                    try
                    {
                        payment.Refund(payment.Amount);
                    }
                    catch (InvalidOperationException)
                    {
                        // Already refunded or partially refunded - ignore
                    }

                    // Reverse wallet credit
                    var wallet = await uow.Wallets.GetByCompanyAsync(sub.SellerCompanyId, ct);
                    if (wallet != null)
                    {
                        // Try to reverse from pending first, then available
                        var amountToReverse = sub.TotalAmount;
                        var fromPending = Math.Min(amountToReverse, wallet.PendingBalance);
                        if (fromPending > 0)
                        {
                            wallet.ReversePending(fromPending);
                            amountToReverse -= fromPending;
                        }
                        if (amountToReverse > 0)
                        {
                            wallet.DebitAvailable(amountToReverse);
                        }
                    }
                }
            }

            sub.Cancel(reason);
        }
    }
}
