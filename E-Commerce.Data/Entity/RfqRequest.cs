using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Identity;
using E_Commerce.Data.Status;

namespace E_Commerce.Data.Entity
{
    public class RfqRequest : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public string Currency { get; private set; } = "EGP";
        public string? TargetPrice { get; private set; }   // Optional target price from buyer
        public string? Attachments { get; private set; }   // Comma-separated URLs
        public string? ShippingCountry { get; private set; }
        public DateTime? DeadlineDate { get; private set; }
        public RfqStatus Status { get; private set; } = RfqStatus.Pending;

        // Relations
        public Guid BuyerId { get; private set; }
        public User Buyer { get; private set; } = null!;

        public Guid SellerCompanyId { get; private set; }
        public Company SellerCompany { get; private set; } = null!;

        public Guid? ProductId { get; private set; }   // Optional: linked product
        public Product? Product { get; private set; }

        public ICollection<RfqQuotation> Quotes { get; private set; } = new List<RfqQuotation>();

        private RfqRequest() { }

        public static RfqRequest Create(
            string title, string description, int quantity,
            Guid buyerId, Guid sellerCompanyId,
            string currency = "EGP",
            string? targetPrice = null,
            string? shippingCountry = null,
            DateTime? deadline = null,
            Guid? productId = null)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");

            return new RfqRequest
            {
                Title = title,
                Description = description,
                Quantity = quantity,
                BuyerId = buyerId,
                SellerCompanyId = sellerCompanyId,
                Currency = currency.ToUpper(),
                TargetPrice = targetPrice,
                ShippingCountry = shippingCountry,
                DeadlineDate = deadline,
                ProductId = productId
            };
        }

        public void Cancel()
        {
            if (Status is RfqStatus.Closed or RfqStatus.Accepted)
                throw new InvalidOperationException("Cannot cancel a closed or accepted RFQ.");
            Status = RfqStatus.Cancelled;
            MarkAsUpdated();
        }

        public void MarkQuoted() { Status = RfqStatus.Quoted; MarkAsUpdated(); }
        public void MarkAccepted() { Status = RfqStatus.Accepted; MarkAsUpdated(); }
        public void MarkDeclined() { Status = RfqStatus.Declined; MarkAsUpdated(); }
        public void MarkClosed() { Status = RfqStatus.Closed; MarkAsUpdated(); }
    }
}
