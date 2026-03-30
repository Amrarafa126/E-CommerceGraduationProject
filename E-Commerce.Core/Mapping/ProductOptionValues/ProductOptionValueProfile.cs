
using AutoMapper;

namespace E_Commerce.Core.Mapping.ProductOptionValues
{
    public partial class ProductOptionValueProfile : Profile
    {
        public ProductOptionValueProfile()
        {
            AddOptionValueCommandMapping();
        }
    }
}
