
using E_Commerce.Core.Features.Shippings;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Mapping.Shippings
{
    public partial class ShippingsProfile 
    {
        public void AddShippingMapping()
        {
            CreateMap<Shipping, ShipmentDto>()
         .ForMember(d => d.Method, o => o.MapFrom(s => (int)s.Method - 1))
         .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status - 1))
         .ForMember(d => d.Events, o => o.MapFrom(s =>
             s.Events.OrderBy(e => e.CreatedAt).ToList()));

            CreateMap<ShippingEvent, ShipmentEventDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status - 1));
        }
    }
}
