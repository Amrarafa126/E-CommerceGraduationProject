namespace E_Commerce.Data.Entity
{
    public class ProductVariantOptionValue : BaseEntity
    {
        public Guid ProductVariantId { get; private set; }
        public ProductVariant ProductVariant { get; private set; } = null!;
        public Guid ProductOptionValueId { get; private set; }
        public ProductOptionValue ProductOptionValue { get; private set; } = null!;

        private ProductVariantOptionValue() { }

        public static ProductVariantOptionValue Create(Guid variantId, Guid optionValueId)
            => new() { ProductVariantId = variantId, ProductOptionValueId = optionValueId };
    }

}
