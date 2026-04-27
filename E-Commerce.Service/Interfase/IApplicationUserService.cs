using E_Commerce.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Service.Interfase
{
    public interface IApplicationUserService
    {
        public Task<string> AddUserAsync(User user, string password);

    }
}
