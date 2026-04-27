using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class RfqQuotation : BaseEntity
    {
        public Guid RfqRequestId { get; private set; }
        public RfqRequest RfqRequest { get; private set; } = null!;

        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public string Currency { get; private set; } = "USD";
        public string? Notes { get; private set; }
        public string? PaymentTerms { get; private set; }
        public string? DeliveryTerms { get; private set; }
        public int ValidityDays { get; private set; } = 7;
        public DateTime ValidUntil { get; private set; }
        public bool IsAccepted { get; private set; }
        public bool IsDeclined { get; private set; }

        private RfqQuotation() { }

        public static RfqQuotation Create(
            Guid rfqRequestId, decimal unitPrice, int quantity,
            string currency = "USD",
            string? notes = null,
            string? paymentTerms = null,
            string? deliveryTerms = null,
            int validityDays = 7)
        {
            if (unitPrice < 0) throw new ArgumentException("Unit price cannot be negative.");
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");

            return new RfqQuotation
            {
                RfqRequestId = rfqRequestId,
                UnitPrice = unitPrice,
                Quantity = quantity,
                Currency = currency.ToUpper(),
                Notes = notes,
                PaymentTerms = paymentTerms,
                DeliveryTerms = deliveryTerms,
                ValidityDays = validityDays,
                ValidUntil = DateTime.UtcNow.AddDays(validityDays)
            };
        }

        public void Accept() { IsAccepted = true; MarkAsUpdated(); }
        public void Decline() { IsDeclined = true; MarkAsUpdated(); }
        public bool IsExpired => DateTime.UtcNow > ValidUntil;
    }
}
