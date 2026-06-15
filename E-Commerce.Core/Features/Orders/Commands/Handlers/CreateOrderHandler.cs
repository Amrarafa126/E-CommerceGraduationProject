using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Orders.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using E_Commerce.Data.ValueObjects;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Orders.Commands.Handlers
{
    public class CreateOrderHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper,
        E_Commerce.Service.Shipping.IShippingRateService rateService)
        : IRequestHandler<CreateOrderCommand, ApiResponse<OrderDto>>
    {
        public async Task<ApiResponse<OrderDto>> Handle(CreateOrderCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var seller = await uow.Companies.GetByIdAsync(req.SellerCompanyId, ct)
                ?? throw new NotFoundException(nameof(Company), req.SellerCompanyId);

            if (!seller.IsActive)
                throw new BusinessException("لا يمكن تقديم طلب لشركة غير نشطة.");

            // Check idempotency key first
            if (!string.IsNullOrWhiteSpace(req.IdempotencyKey))
            {
                var existingOrder = await uow.Orders.FirstOrDefaultAsync(
                    o => o.IdempotencyKey == req.IdempotencyKey, ct);
                if (existingOrder != null)
                {
                    var existingDto = mapper.Map<OrderDto>(existingOrder);
                    return ApiResponse<OrderDto>.Ok(existingDto);
                }
            }

            await uow.BeginTransactionAsync(ct);
            try
            {
                // Create parent order
                var order = Order.Create(cu.UserId.Value, req.Notes, req.Currency, req.PoNumber);
                if (!string.IsNullOrWhiteSpace(req.IdempotencyKey))
                    order.SetIdempotencyKey(req.IdempotencyKey);

                // Create single sub-order for this seller
                var subOrder = OrderSubOrder.Create(order.Id, req.SellerCompanyId, req.Currency);

                foreach (var itemReq in req.Items)
                {
                    var product = await uow.Products.GetWithFullDetailsAsync(itemReq.ProductId, ct)
                        ?? throw new NotFoundException(nameof(Product), itemReq.ProductId);

                    if (product.CompanyId != req.SellerCompanyId)
                        throw new BusinessException($"Product '{product.Name}' لا يتبع البائع المختار.");

                    if (product.Status != ProductStatus.Active)
                        throw new BusinessException($"Product '{product.Name}' غير متاح.");

                    if (itemReq.Quantity < product.MinimumOrderQuantity)
                        throw new BusinessException(
                            $"أقل كمية للطلب لـ '{product.Name}' is {product.MinimumOrderQuantity}.");

                    decimal unitPrice;
                    string? variantSku = null;
                    string? variantName = null;
                    decimal originalBasePrice = product.BasePrice;
                    bool priceTierApplied = false;
                    int? priceTierMinQty = null;

                    if (itemReq.ProductVariantId.HasValue)
                    {
                        var variant = product.productVariants
                            .FirstOrDefault(v => v.Id == itemReq.ProductVariantId && v.IsActive)
                            ?? throw new NotFoundException(nameof(ProductVariant), itemReq.ProductVariantId.Value);

                        if (variant.StockQuantity < itemReq.Quantity)
                            throw new BusinessException(
                                $"المخزون غير كافٍ لـ SKU '{variant.SKU}'. المتاح: {variant.StockQuantity}.");

                        variant.AdjustStock(-itemReq.Quantity);
                        unitPrice = variant.Price;
                        variantSku = variant.SKU;
                        variantName = string.Join(", ", variant.OptionValues.Select(ov => ov.ProductOptionValue?.Value ?? ""));
                    }
                    else if (product.productVariants.Any(v => v.IsActive))
                    {
                        throw new BusinessException($"يجب اختيار متغير للمنتج '{product.Name}'.");
                    }
                    else
                    {
                        // Check stock for non-variant products
                        if (product.StockQuantity < itemReq.Quantity)
                            throw new BusinessException(
                                $"المخزون غير كافٍ لـ '{product.Name}'. المتاح: {product.StockQuantity}.");

                        product.AdjustStock(-itemReq.Quantity);

                        unitPrice = product.GetPriceForQuantity(itemReq.Quantity);
                        priceTierApplied = unitPrice != product.BasePrice;
                        if (priceTierApplied)
                        {
                            var tier = product.PriceTiers
                                .Where(t => itemReq.Quantity >= t.MinQuantity)
                                .OrderByDescending(t => t.MinQuantity)
                                .FirstOrDefault();
                            priceTierMinQty = tier?.MinQuantity;
                        }
                    }

                    var orderItem = OrderItem.Create(
                        subOrder.Id, product.Id, product.Name ?? "غير معروف",
                        itemReq.Quantity, unitPrice,
                        itemReq.ProductVariantId, variantSku,
                        product.MainImageUrl,
                        product.Description?[..Math.Min(product.Description?.Length ?? 0, 500)],
                        product.Category?.Name,
                        originalBasePrice,
                        priceTierApplied,
                        priceTierMinQty,
                        seller.CompanyName,
                        variantName);

                    subOrder.AddItem(orderItem);
                }

                order.AddSubOrder(subOrder);

                // Validate and apply shipping details
                var shippingMethod = (ShippingMethod)(req.ShippingMethod + 1);
                if (!Enum.IsDefined(typeof(ShippingMethod), shippingMethod))
                    throw new ValidationException("طريقة الشحن غير صالحة.");

                var addr = req.ShippingAddress;
                if (string.IsNullOrWhiteSpace(addr?.RecipientName) ||
                    string.IsNullOrWhiteSpace(addr.PhoneNumber) ||
                    string.IsNullOrWhiteSpace(addr.AddressLine1) ||
                    string.IsNullOrWhiteSpace(addr.City) ||
                    string.IsNullOrWhiteSpace(addr.Country))
                    throw new ValidationException("عنوان الشحن غير مكتمل.");

                var sellerCompany = await uow.Companies.GetByIdAsync(req.SellerCompanyId, ct)
                    ?? throw new NotFoundException(nameof(Company), req.SellerCompanyId);

                var pickupCity = sellerCompany.Address?.City ?? addr.City;
                var pickupState = sellerCompany.Address?.State ?? addr.State;

                var rateEstimate = await rateService.EstimateAsync(
                    addr.Country, addr.City, addr.State,
                    pickupCity, pickupState,
                    shippingMethod, ct);

                subOrder.SetShippingDetails(shippingMethod, rateEstimate.Cost);
                order.SetShippingAddress(new ShippingAddress(
                    addr.RecipientName, addr.PhoneNumber, addr.AddressLine1,
                    addr.City, addr.State, addr.Country, addr.PostalCode, addr.AddressLine2));
                order.RecalculateTotals();

                await uow.Orders.AddAsync(order, ct);
                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                var full = await uow.Orders.GetWithFullDetailsAsync(order.Id, ct);
                var dto = mapper.Map<OrderDto>(full!);
                return ApiResponse<OrderDto>.Created(dto);
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
    }
}
