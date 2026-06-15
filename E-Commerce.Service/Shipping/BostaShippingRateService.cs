using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace E_Commerce.Service.Shipping
{
    public class BostaShippingRateService : IShippingRateService
    {
        private readonly IBostaClient _bostaClient;
        private readonly IOptions<BostaOptions> _options;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<BostaShippingRateService> _logger;

        public BostaShippingRateService(
            IBostaClient bostaClient,
            IOptions<BostaOptions> options,
            IUnitOfWork uow,
            ILogger<BostaShippingRateService> logger)
        {
            _bostaClient = bostaClient;
            _options = options;
            _uow = uow;
            _logger = logger;
        }

        public async Task<ShippingRateEstimate> EstimateAsync(
            string country,
            string city,
            string? state,
            string pickupCity,
            string? pickupState,
            ShippingMethod method,
            CancellationToken ct = default)
        {
            if (!string.IsNullOrWhiteSpace(_options.Value.BaseUrl) && _options.Value.IsTestMode == false)
            {
                try
                {
                    var price = await _bostaClient.GetDeliveryPriceAsync(
                        new BostaPriceRequest(
                            new BostaAddress("Seller Warehouse", pickupCity, pickupState ?? city),
                            new BostaAddress(city, city, state ?? city),
                            new BostaSpecs(1, "Parcel", "1")),
                        ct);

                    if (price != null)
                    {
                        return new ShippingRateEstimate(
                            price.deliveryFees,
                            "EGP",
                            price.estimatedDays,
                            "Bosta",
                            true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Bosta live pricing failed; falling back to internal rate table.");
                }
            }

            return await GetFallbackEstimateAsync(country, city, state, method, ct);
        }

        private async Task<ShippingRateEstimate> GetFallbackEstimateAsync(
            string country,
            string city,
            string? state,
            ShippingMethod method,
            CancellationToken ct)
        {
            var zone = city;
            var rate = await _uow.ShippingRates
                .FirstOrDefaultAsync(r =>
                    r.IsActive &&
                    r.Country == country &&
                    r.Method == method &&
                    (r.Zone == zone || r.Zone == state),
                ct);

            if (rate != null)
            {
                return new ShippingRateEstimate(rate.Cost, rate.Currency, rate.EstimatedDays, "Internal", false);
            }

            var (cost, days) = method switch
            {
                ShippingMethod.Standard => (50m, 5),
                ShippingMethod.Express => (100m, 3),
                ShippingMethod.Overnight => (200m, 1),
                ShippingMethod.Pickup => (0m, 0),
                _ => (50m, 5)
            };

            return new ShippingRateEstimate(cost, "EGP", days, "Internal", false);
        }
    }
}
