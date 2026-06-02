using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Queries.Models;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Payment.Queries.Handlers
{
    public class GetPaymentByOrderHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<GetPaymentByOrderQuery, ApiResponse<PaymentDto>>
    {
        public async Task<ApiResponse<PaymentDto>> Handle(GetPaymentByOrderQuery req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            // For backward compatibility, req.OrderId might be a parent Order ID or SubOrder ID
            // Try to find payment by sub-order ID first, then by parent order's sub-order
            var payment = await uow.payment.GetByOrderSubOrderIdAsync(req.OrderId, ct);

            if (payment == null)
            {
                // Maybe the ID is a parent order ID - find its sub-order's payment
                var order = await uow.Orders.GetWithFullDetailsAsync(req.OrderId, ct);
                var subOrder = order?.SubOrders.FirstOrDefault();
                if (subOrder != null)
                    payment = await uow.payment.GetByOrderSubOrderIdAsync(subOrder.Id, ct);
            }

            if (payment == null)
                throw new NotFoundException("الدفع للطلب", req.OrderId);

            // Fix: get sub-order first to find parent OrderId
            var paymentSubOrder = await uow.OrderSubOrders.GetByIdAsync(payment.OrderSubOrderId, ct);
            if (paymentSubOrder == null)
                throw new NotFoundException("الطلب الفرعي", payment.OrderSubOrderId);

            var orderFull = await uow.Orders.GetWithFullDetailsAsync(paymentSubOrder.OrderId, ct)
                ?? throw new NotFoundException("الطلب", paymentSubOrder.OrderId);

            bool isBuyer = orderFull.BuyerId == cu.UserId.Value;
            bool isSeller = cu.OwnedCompanyId != null && orderFull.SubOrders.Any(s => s.SellerCompanyId == cu.OwnedCompanyId.Value);
            bool isAdmin = cu.Role == "Admin";

            if (!isBuyer && !isSeller && !isAdmin)
                throw new ForbiddenException("يمكنك عرض المدفوعات فقط لطلباتك.");

            var dto = mapper.Map<PaymentDto>(payment);
            return ApiResponse<PaymentDto>.Ok(dto);
        }
    }
}
