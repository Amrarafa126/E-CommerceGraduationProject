using AutoMapper;
using E_Commerce.Core.Features.Orders;
using E_Commerce.Data.Entity;
using E_Commerce.Data.ValueObjects;

namespace E_Commerce.Core.Mapping.Orders
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OverallStatus, opt => opt.MapFrom(src => (int)src.OverallStatus - 1))
                .ForMember(dest => dest.BuyerName, opt => opt.MapFrom(src => src.Buyer != null ? src.Buyer.FullName : ""))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress));

            CreateMap<ShippingAddress, ShippingAddressDto>();

            CreateMap<OrderSubOrder, OrderSubOrderDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.BuyerId, opt => opt.MapFrom(src => src.Order != null ? src.Order.BuyerId : Guid.Empty))
                .ForMember(dest => dest.BuyerName, opt => opt.MapFrom(src => src.Order != null && src.Order.Buyer != null ? src.Order.Buyer.FullName : ""))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status - 1))
                .ForMember(dest => dest.SellerCompanyName, opt => opt.MapFrom(src => src.SellerCompany != null ? src.SellerCompany.CompanyName : ""))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Payment != null ? (int)src.Payment.Status - 1 : (int?)null))
                .ForMember(dest => dest.ShipmentStatus, opt => opt.MapFrom(src => src.Shipment != null ? (int)src.Shipment.Status - 1 : (int?)null))
                .ForMember(dest => dest.ShippingMethod, opt => opt.MapFrom(src => (int)src.ShippingMethod - 1))
                .ForMember(dest => dest.BostaTrackingNumber, opt => opt.MapFrom(src => src.Shipment != null ? src.Shipment.BostaTrackingNumber : null))
                .ForMember(dest => dest.BostaShipmentId, opt => opt.MapFrom(src => src.Shipment != null ? src.Shipment.BostaShipmentId : null))
                .ForMember(dest => dest.StatusHistory, opt => opt.MapFrom(src => src.StatusHistory.OrderBy(h => h.CreatedAt).ToList()));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.LineTotal, opt => opt.MapFrom(src => src.TotalPrice));

            CreateMap<OrderStatusHistory, OrderStatusHistoryDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status - 1));
        }
    }
}
