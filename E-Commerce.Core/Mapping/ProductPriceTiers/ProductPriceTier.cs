using AutoMapper;

namespace E_Commerce.Core.Mapping.ProductPriceTiers
{
    public partial class ProductPriceTierProfile : Profile
    {
        public ProductPriceTierProfile()
        {
            AddPriceTierCommandMapping();
        }
    }
}
