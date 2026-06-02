using AutoMapper;
using E_Commerce.Core.Features.Payment;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Mapping.Payments
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.SubOrderId, opt => opt.MapFrom(src => src.OrderSubOrderId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status - 1))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => (int)src.Method - 1))
                .ForMember(dest => dest.PaymentKey, opt => opt.MapFrom(src => src.PaymobToken));

            CreateMap<Wallet, WalletDto>();

            CreateMap<Payout, PayoutDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status - 1));
        }
    }
}
