using E_Commerce.Data.Identity;
using E_Commerce.Data.Status;

namespace E_Commerce.Data.Entity
{
    public class RfqRequest : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public UnitOfMeasure UnitOfMeasure { get; private set; } = UnitOfMeasure.Piece;
        public string Currency { get; private set; } = "EGP";
        public string? TargetPrice { get; private set; }
        public string? Attachments { get; private set; }
        public string? ShippingCountry { get; private set; }
        public string? DestinationCity { get; private set; }
        public string? DestinationCountry { get; private set; }
        public ShippingMethod? PreferredShippingMethod { get; private set; }
        public string? PaymentTerms { get; private set; }
        public string? RequiredCertifications { get; private set; }
        public string? SupplierRequirements { get; private set; }
        public DateTime? DeadlineDate { get; private set; }
        public bool IsPublic { get; private set; } = true;
        public RfqStatus Status { get; private set; } = RfqStatus.Pending;

        // Relations
        public Guid BuyerId { get; private set; }
        public User Buyer { get; private set; } = null!;

        public Guid? SellerCompanyId { get; private set; }
        public Company? SellerCompany { get; private set; }

        public Guid? CategoryId { get; private set; }
        public Category? Category { get; private set; }

        public Guid? ProductId { get; private set; }
        public Product? Product { get; private set; }

        public ICollection<RfqQuotation> Quotes { get; private set; } = new List<RfqQuotation>();

        private RfqRequest() { }

        public static RfqRequest Create(
            string title, string description, int quantity, Guid buyerId,
            string currency = "EGP",
            UnitOfMeasure? unitOfMeasure = null,
            Guid? sellerCompanyId = null,
            Guid? categoryId = null,
            string? targetPrice = null,
            string? shippingCountry = null,
            string? destinationCity = null,
            string? destinationCountry = null,
            ShippingMethod? preferredShippingMethod = null,
            string? paymentTerms = null,
            string? requiredCertifications = null,
            string? supplierRequirements = null,
            DateTime? deadline = null,
            Guid? productId = null,
            string? attachments = null,
            bool isPublic = true)
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
                CategoryId = categoryId,
                Currency = currency.ToUpper(),
                UnitOfMeasure = unitOfMeasure ?? UnitOfMeasure.Piece,
                TargetPrice = targetPrice,
                Attachments = attachments,
                ShippingCountry = shippingCountry,
                DestinationCity = destinationCity,
                DestinationCountry = destinationCountry,
                PreferredShippingMethod = preferredShippingMethod,
                PaymentTerms = paymentTerms,
                RequiredCertifications = requiredCertifications,
                SupplierRequirements = supplierRequirements,
                DeadlineDate = deadline,
                ProductId = productId,
                IsPublic = isPublic
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
