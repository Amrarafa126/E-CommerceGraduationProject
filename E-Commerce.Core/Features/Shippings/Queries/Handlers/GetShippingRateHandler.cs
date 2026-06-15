using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Shippings.Queries.Models;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using E_Commerce.Service.Shipping;
using MediatR;

namespace E_Commerce.Core.Features.Shippings.Queries.Handlers
{
    public class GetShippingRateHandler(
        IShippingRateService rateService,
        IUnitOfWork uow,
        ICurrentUserService cu)
        : IRequestHandler<GetShippingRateQuery, ApiResponse<ShippingRateEstimate>>
    {
        public async Task<ApiResponse<ShippingRateEstimate>> Handle(GetShippingRateQuery req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var method = (ShippingMethod)(req.Method + 1);
            if (!Enum.IsDefined(typeof(ShippingMethod), method))
                throw new ValidationException("طريقة الشحن غير صالحة.");

            var pickupCity = req.PickupCity;
            var pickupState = req.PickupState;

            if (req.SellerCompanyId.HasValue)
            {
                var company = await uow.Companies.GetByIdAsync(req.SellerCompanyId.Value, ct);
                if (company?.Address != null)
                {
                    pickupCity ??= company.Address.City;
                    pickupState ??= company.Address.State;
                }
            }

            pickupCity ??= req.City;
            pickupState ??= req.State ?? req.City;

            var estimate = await rateService.EstimateAsync(
                req.Country,
                req.City,
                req.State,
                pickupCity,
                pickupState,
                method,
                ct);

            return ApiResponse<ShippingRateEstimate>.Ok(estimate);
        }
    }
}
