

using E_Commerce.Core.Features.Orders;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Mapping.Orders
{
    public partial class OrderProfile
    {
        public void AddMapping()
        {
            CreateMap<Order, OrderDto>()
         .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status - 1))
         .ForMember(d => d.BuyerName, o => o.MapFrom(s => s.Buyer != null ? s.Buyer.FullName : ""))
         .ForMember(d => d.SellerCompanyName, o => o.MapFrom(s => s.SellerCompany != null ? s.SellerCompany.CompanyName : ""))
         .ForMember(d => d.PaymentStatus, o => o.MapFrom(s => s.Payment != null ? (int)s.Payment.Status - 1 : (int?)null))
         .ForMember(d => d.ShipmentStatus, o => o.MapFrom(s => s.Shipment != null ? (int)s.Shipment.Status - 1 : (int?)null))
         .ForMember(d => d.StatusHistory, o => o.MapFrom(s =>
             s.StatusHistory.OrderBy(h => h.CreatedAt).ToList()));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.LineTotal, o => o.MapFrom(s => s.TotalPrice));

            CreateMap<OrderStatusHistory, OrderStatusHistoryDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status - 1));
        }
    }
}
