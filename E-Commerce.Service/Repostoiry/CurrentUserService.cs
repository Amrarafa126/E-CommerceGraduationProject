using E_Commerce.Service.Interfase;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Service.Repostoiry
{
    
        public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
        {
            private readonly ClaimsPrincipal? _user = httpContextAccessor.HttpContext?.User;

            public Guid? UserId
            {
                get
                {
                    var value = _user?.FindFirstValue(ClaimTypes.NameIdentifier);
                    return Guid.TryParse(value, out var id) ? id : null;
                }
            }

            public string? Email => _user?.FindFirstValue(ClaimTypes.Email);
            public string? Role => _user?.FindFirstValue(ClaimTypes.Role);
            public bool IsAuthenticated => _user?.Identity?.IsAuthenticated ?? false;
        }
    }

