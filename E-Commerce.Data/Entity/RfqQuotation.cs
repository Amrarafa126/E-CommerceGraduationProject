using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Data.Entity
{
    public class RfqQuotation : BaseEntity
    {
        public Guid RfqRequestId { get; private set; }
        public RfqRequest RfqRequest { get; private set; } = null!;

        public Guid SellerCompanyId { get; private set; }
        public Company SellerCompany { get; private set; } = null!;

        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        [NotMapped]
        public decimal TotalPrice => UnitPrice * Quantity;
        public string Currency { get; private set; } = "EGP";
        public string? Notes { get; private set; }
        public string? PaymentTerms { get; private set; }
        public string? DeliveryTerms { get; private set; }
        public int ValidityDays { get; private set; } = 7;
        public DateTime ValidUntil { get; private set; }
        public int? LeadTimeDays { get; private set; }
        public bool SampleAvailable { get; private set; }
        public bool IsAccepted { get; private set; }
        public bool IsDeclined { get; private set; }

        private RfqQuotation() { }

        public static RfqQuotation Create(
            Guid rfqRequestId, Guid sellerCompanyId, decimal unitPrice, int quantity,
            string currency = "EGP",
            string? notes = null,
            string? paymentTerms = null,
            string? deliveryTerms = null,
            int validityDays = 7,
            int? leadTimeDays = null,
            bool sampleAvailable = false)
        {
            if (unitPrice < 0) throw new ArgumentException("Unit price cannot be negative.");
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");

            return new RfqQuotation
            {
                RfqRequestId = rfqRequestId,
                SellerCompanyId = sellerCompanyId,
                UnitPrice = unitPrice,
                Quantity = quantity,
                Currency = currency.ToUpper(),
                Notes = notes,
                PaymentTerms = paymentTerms,
                DeliveryTerms = deliveryTerms,
                ValidityDays = validityDays,
                ValidUntil = DateTime.UtcNow.AddDays(validityDays),
                LeadTimeDays = leadTimeDays,
                SampleAvailable = sampleAvailable
            };
        }

        public void Accept() { IsAccepted = true; MarkAsUpdated(); }
        public void Decline() { IsDeclined = true; MarkAsUpdated(); }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow > ValidUntil;
    }
}
