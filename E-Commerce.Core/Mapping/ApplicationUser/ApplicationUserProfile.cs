using AutoMapper;
namespace E_Commerce.Core.Mapping.ApplicationUser
{
    public partial class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            AddUserMapping();
            UpdateUserMapping();
        }
    }
}
