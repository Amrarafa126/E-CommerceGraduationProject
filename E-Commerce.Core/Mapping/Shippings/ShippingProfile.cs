using AutoMapper;
using E_Commerce.Core.Features.Shippings;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Mapping.Shippings
{
    public class ShippingProfile : Profile
    {
        public ShippingProfile()
        {
            CreateMap<Shipping, ShipmentDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderSubOrderId))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => (int)src.Method - 1))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status - 1))
                .ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.Events.OrderBy(e => e.CreatedAt).ToList()));

            CreateMap<ShippingEvent, ShipmentEventDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status - 1));
        }
    }
}
