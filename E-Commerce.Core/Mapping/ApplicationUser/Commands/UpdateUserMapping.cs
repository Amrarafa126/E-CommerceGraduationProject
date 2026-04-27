

using E_Commerce.Core.Features.ApplicationUser.Commands.Models;
using E_Commerce.Data.Identity;

namespace E_Commerce.Core.Mapping.ApplicationUser
{
    public partial class ApplicationUserProfile
    {
        public void UpdateUserMapping()
        {
            CreateMap<EditUserCommand, User>();
        }
    }
}
