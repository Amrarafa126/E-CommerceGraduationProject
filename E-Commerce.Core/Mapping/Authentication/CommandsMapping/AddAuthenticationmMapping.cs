using AutoMapper;
using E_Commerce.Core.Features.Authentication;
using E_Commerce.Core.Features.Companies;
using E_Commerce.Data.Identity;


namespace E_Commerce.Core.Mapping.Authentication
{
    public partial class AuthenticationProfile 
    {
        public void AddMappingAuthenticationProfile()
        {
            CreateMap<User, UserDto>()
          .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
          .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()))
          .ForMember(d => d.OwnedCompanyId, o => o.MapFrom(s => s.OwnedCompanyId))
          .ForMember(d => d.IsEmailVerified, o => o.MapFrom(s => s.EmailConfirmed));
        }
    }
}
