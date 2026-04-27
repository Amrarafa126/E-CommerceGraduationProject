using E_Commerce.Core.Features.ProductOptionValues;
using E_Commerce.Core.Features.ProductOptionValues.Commands.Models;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Mapping.ProductOptionValues
{
    public partial class ProductOptionValueProfile
    {
        public void AddOptionValueCommandMapping()
        {
            CreateMap<ProductOptionValue, ProductOptionValueDto>();
        }
    }
}
