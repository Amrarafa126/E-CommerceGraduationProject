
using AutoMapper;
using E_Commerce.Core.Features.Companies.Commands.Models;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Mapping.Companies
{
    public partial class CampanyProfile : Profile
    {
        public void AddCompanyMapping()
        {
            CreateMap<AddCompanyCommands, Company>();

        }
    }
}
