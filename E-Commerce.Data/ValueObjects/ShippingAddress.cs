using E_Commerce.Data.Entity;

namespace E_Commerce.Data.ValueObjects
{
    public sealed class ShippingAddress : BaseEntity
    {
        public string RecipientName { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public string AddressLine1 { get; private set; } = string.Empty;
        public string? AddressLine2 { get; private set; }
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;
        public string Country { get; private set; } = string.Empty;
        public string PostalCode { get; private set; } = string.Empty;

        private ShippingAddress() { }

        public ShippingAddress(
            string recipientName,
            string phoneNumber,
            string addressLine1,
            string city,
            string state,
            string country,
            string postalCode,
            string? addressLine2 = null)
        {
            if (string.IsNullOrWhiteSpace(recipientName))
                throw new ArgumentException("Recipient name is required.", nameof(recipientName));
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.", nameof(phoneNumber));
            if (string.IsNullOrWhiteSpace(addressLine1))
                throw new ArgumentException("Address line 1 is required.", nameof(addressLine1));
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City is required.", nameof(city));
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country is required.", nameof(country));

            RecipientName = recipientName;
            PhoneNumber = phoneNumber;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            City = city;
            State = state;
            Country = country;
            PostalCode = postalCode;
        }

        public override string ToString() =>
            $"{RecipientName}, {AddressLine1}{(AddressLine2 != null ? ", " + AddressLine2 : "")}, {City}, {State}, {Country} {PostalCode}";
    }
}
