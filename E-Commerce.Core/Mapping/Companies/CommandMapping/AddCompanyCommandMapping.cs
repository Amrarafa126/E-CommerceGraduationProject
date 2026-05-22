
using AutoMapper;
using E_Commerce.Core.Features.Companies;
using E_Commerce.Core.Features.Companies.Commands.Models;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Mapping.Companies
{
    public partial class CampanyProfile : Profile
    {
        public void AddCompanyMapping()
        {
            CreateMap<Company, CompanyDto>()
           .ForMember(d => d.Name, o => o.MapFrom(s => s.CompanyName ?? ""))
           .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status - 1))
           .ForMember(d => d.OwnerName, o => o.MapFrom(s => s.Owner != null ? s.Owner.FullName : ""))
           .ForMember(d => d.Street, o => o.MapFrom(s => s.Address.Street))
           .ForMember(d => d.City, o => o.MapFrom(s => s.Address.City))
           .ForMember(d => d.State, o => o.MapFrom(s => s.Address.State))
           .ForMember(d => d.Country, o => o.MapFrom(s => s.Address.Country))
           .ForMember(d => d.PostalCode, o => o.MapFrom(s => s.Address.PostalCode))
           .ForMember(d => d.ContactEmail, o => o.MapFrom(s => s.ContactInfo.Email))
           .ForMember(d => d.ContactPhone, o => o.MapFrom(s => s.ContactInfo.Phone))
           .ForMember(d => d.WalletPendingBalance, o => o.MapFrom(s => s.Wallet != null ? s.Wallet.PendingBalance : 0m))
           .ForMember(d => d.WalletAvailableBalance, o => o.MapFrom(s => s.Wallet != null ? s.Wallet.AvailableBalance : 0m));

            CreateMap<Company, CompanySummaryDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.CompanyName ?? ""))
                .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status - 1))
                .ForMember(d => d.City, o => o.MapFrom(s => s.Address.City))
                .ForMember(d => d.Country, o => o.MapFrom(s => s.Address.Country));


        }
    }
}
