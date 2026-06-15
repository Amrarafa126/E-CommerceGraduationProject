using E_Commerce.Data.Status;

namespace E_Commerce.Data.Entity
{
    public class ShippingRate : BaseEntity
    {
        public string Zone { get; private set; } = string.Empty; // city/state/region name
        public string Country { get; private set; } = "Egypt";
        public ShippingMethod Method { get; private set; } = ShippingMethod.Standard;
        public decimal Cost { get; private set; }
        public string Currency { get; private set; } = "EGP";
        public int EstimatedDays { get; private set; }
        public bool IsActive { get; private set; } = true;

        private ShippingRate() { }

        public static ShippingRate Create(
            string zone,
            ShippingMethod method,
            decimal cost,
            string country = "Egypt",
            string currency = "EGP",
            int estimatedDays = 5)
        {
            if (string.IsNullOrWhiteSpace(zone))
                throw new ArgumentException("Zone is required.", nameof(zone));
            if (cost < 0)
                throw new ArgumentException("Cost cannot be negative.", nameof(cost));

            return new ShippingRate
            {
                Zone = zone.Trim(),
                Method = method,
                Cost = cost,
                Country = country.Trim(),
                Currency = currency.ToUpper(),
                EstimatedDays = estimatedDays
            };
        }

        public void UpdateCost(decimal cost, int estimatedDays)
        {
            if (cost < 0) throw new ArgumentException("Cost cannot be negative.");
            Cost = cost;
            EstimatedDays = estimatedDays;
            MarkAsUpdated();
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}
