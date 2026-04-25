
using E_Commerce.Core.Features.ProductOptions;
using E_Commerce.Core.Features.ProductOptions.Commands.Models;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Mapping.ProductOptions
{
    public partial class ProductOptionProfile 
    {
        public void AddProductOptionCommandMapping()
        {
            CreateMap<ProductOption, ProductOptionDto>();



        }
    }
}
