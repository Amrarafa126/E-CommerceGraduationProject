using AutoMapper;

namespace E_Commerce.Core.Mapping.Shippings
{
    public partial class ShippingsProfile : Profile
    {
        public ShippingsProfile()
        {
            AddShippingMapping();
        }
    }
}
