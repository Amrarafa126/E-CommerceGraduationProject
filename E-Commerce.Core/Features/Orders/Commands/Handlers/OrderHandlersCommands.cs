using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Orders.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;


namespace E_Commerce.Core.Features.Orders.Commands.Handlers
{
    public class OrderHandlersCommands(IUnitOfWork uow, ICurrentUserService cu)
    : IRequestHandler<CreateOrderCommand, ApiResponse<OrderDto>>,
      IRequestHandler<CompleteOrderCommand, ApiResponse<OrderDto>>,
      IRequestHandler<CancelOrderCommand, ApiResponse<OrderDto>>
    {
        public async Task<ApiResponse<OrderDto>> Handle(CreateOrderCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var seller = await uow.Companies.GetByIdAsync(req.SellerCompanyId, ct)
                ?? throw new NotFoundException(nameof(Company), req.SellerCompanyId);

            if (!seller.IsActive)
                throw new BusinessException("Cannot place order with an inactive company.");

            await uow.BeginTransactionAsync(ct);
            try
            {
                var order = Order.Create(cu.UserId.Value, req.SellerCompanyId,
                    req.Notes, req.Currency);

                await uow.Orders.AddAsync(order, ct);
                await uow.SaveChangesAsync(ct); 

                foreach (var itemReq in req.Items)
                {
                    var product = await uow.Products.GetWithFullDetailsAsync(itemReq.ProductId, ct)
                        ?? throw new NotFoundException(nameof(Product), itemReq.ProductId);

                    if (product.CompanyId != req.SellerCompanyId)
                        throw new BusinessException(
                            $"Product '{product.Name}' does not belong to the selected seller.");

                    if (product.Status != ProductStatus.Active)
                        throw new BusinessException($"Product '{product.Name}' is not available.");

                    if (itemReq.Quantity < product.MinimumOrderQuantity)
                        throw new BusinessException(
                            $"Minimum order quantity for '{product.Name}' is {product.MinimumOrderQuantity}.");

                    decimal unitPrice;
                    string? variantSku = null;

                    if (itemReq.ProductVariantId.HasValue)
                    {
                        var variant = product.productVariants
                            .FirstOrDefault(v => v.Id == itemReq.ProductVariantId && v.IsActive)
                            ?? throw new NotFoundException(nameof(ProductVariant), itemReq.ProductVariantId.Value);

                        if (variant.StockQuantity < itemReq.Quantity)
                            throw new BusinessException(
                                $"Insufficient stock for SKU '{variant.SKU}'. Available: {variant.StockQuantity}.");

                        variant.AdjustStock(-itemReq.Quantity);
                        unitPrice = variant.Price;
                        variantSku = variant.SKU;
                    }
                    else
                    {
                        unitPrice = product.GetPriceForQuantity(itemReq.Quantity);
                    }

                    var orderItem = OrderItem.Create(
                        order.Id, product.Id, product.Name,
                        itemReq.Quantity, unitPrice,
                        itemReq.ProductVariantId, variantSku);

                    order.AddItem(orderItem);
                }
                order.RecalculateTotals();
                uow.Orders.Update(order);
                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                var full = await uow.Orders.GetWithFullDetailsAsync(order.Id, ct);
                return ApiResponse<OrderDto>.Created(MapExt.MapOrder(full!));
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }

        public async Task<ApiResponse<OrderDto>> Handle(CompleteOrderCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var order = await uow.Orders.GetWithFullDetailsAsync(req.OrderId, ct)
                ?? throw new NotFoundException(nameof(Order), req.OrderId);

            if (order.BuyerId != cu.UserId.Value && cu.Role != "Admin")
                throw new ForbiddenException("Only the buyer or admin can complete an order.");

            order.MarkCompleted();
            uow.Orders.Update(order);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<OrderDto>.Ok(MapExt.MapOrder(order));
        }

        public async Task<ApiResponse<OrderDto>> Handle(CancelOrderCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var order = await uow.Orders.GetWithFullDetailsAsync(req.OrderId, ct)
                ?? throw new NotFoundException(nameof(Order), req.OrderId);

            bool isBuyer = order.BuyerId == cu.UserId.Value;
            bool isSeller = cu.OwnedCompanyId == order.SellerCompanyId; //***********************************************
            bool isAdmin = cu.Role == "Admin";

            if (!isBuyer && !isSeller && !isAdmin)
                throw new ForbiddenException("You are not authorized to cancel this order.");

            order.Cancel(req.Reason);
            uow.Orders.Update(order);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<OrderDto>.Ok(MapExt.MapOrder(order));
        }
    }
}
