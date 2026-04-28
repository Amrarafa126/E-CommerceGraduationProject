
using E_Commerce.Core.Features.ProductImages;
using E_Commerce.Core.Features.ProductOptions;
using E_Commerce.Core.Features.ProductPriceTiers;
using E_Commerce.Core.Features.Products;
using E_Commerce.Core.Features.ProductVariants;
using E_Commerce.Data.Entity;


namespace E_Commerce.Core.Mapping.Products
{
    public partial class ProductsProfile
    {
            
        public void AddProductMapping()
        {
            CreateMap<Product, ProductSummaryDto>()
           .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
           .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company != null ? s.Company.CompanyName : null))
           .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : null));

            CreateMap<Product, ProductDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company != null ? s.Company.CompanyName : null))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : null))
                .ForMember(d => d.Images, o => o.MapFrom(s =>
                    s.Images.Where(i => !i.IsDeleted).OrderBy(i => i.DisplayOrder).ToList()))
                .ForMember(d => d.Options, o => o.MapFrom(s =>
                    s.ProductOptions.OrderBy(op => op.DisplayOrder).ToList()))
                .ForMember(d => d.PriceTiers, o => o.MapFrom(s =>
                    s.PriceTiers.OrderBy(t => t.MinQuantity).ToList()));

            CreateMap<ProductImage, ProductImageDto>();
            CreateMap<ProductPriceTier, PriceTierDto>();

            CreateMap<ProductOption, ProductOptionDto>();
                

            CreateMap<ProductOptionValue, ProductOptionValueDto>();

            CreateMap<ProductVariant, ProductVariantDto>()
                .ForMember(d => d.OptionValues, o => o.MapFrom(s =>
                    s.OptionValues.Select(ov => ov.ProductOptionValue).ToList()));

            // ProductOptionValue → VariantOptionValueDto needs OptionName from parent
            CreateMap<ProductOptionValue, VariantOptionValueDto>()
                .ForMember(d => d.OptionName, o => o.MapFrom(s =>
                    s.ProductOption != null ? s.ProductOption.Name : ""));

        }



    }
}
