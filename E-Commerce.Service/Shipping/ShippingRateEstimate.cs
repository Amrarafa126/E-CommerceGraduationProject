namespace E_Commerce.Service.Shipping
{
    public record ShippingRateEstimate(
        decimal Cost,
        string Currency,
        int EstimatedDays,
        string Provider,
        bool IsLiveRate);
}
