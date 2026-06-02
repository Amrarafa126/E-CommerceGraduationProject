using AutoMapper;
using E_Commerce.Core.Features.RFQ;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Mapping.RFQs
{
    public partial class RFQProfile
    {
        public void AddRFQMapping()
        {
            CreateMap<RfqRequest, RfqRequestDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status))
                .ForMember(d => d.UnitOfMeasure, o => o.MapFrom(s => (int)s.UnitOfMeasure))
                .ForMember(d => d.PreferredShippingMethod, o => o.MapFrom(s => (int?)s.PreferredShippingMethod))
                .ForMember(d => d.BuyerName, o => o.MapFrom(s => s.Buyer != null ? s.Buyer.FullName : ""))
                .ForMember(d => d.SellerCompanyName, o => o.MapFrom(s => s.SellerCompany != null ? s.SellerCompany.CompanyName : null))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : null))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : null));

            CreateMap<RfqQuotation, RfqQuoteDto>()
                .ForMember(d => d.TotalPrice, o => o.MapFrom(s => s.TotalPrice))
                .ForMember(d => d.IsExpired, o => o.MapFrom(s => s.IsExpired))
                .ForMember(d => d.SellerCompanyName, o => o.MapFrom(s => s.SellerCompany != null ? s.SellerCompany.CompanyName : ""));
        }
    }
}
