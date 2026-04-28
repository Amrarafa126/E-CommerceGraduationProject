using E_Commerce.Service.Interfase;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace E_Commerce.Service.Repostoiry
{
    
        public class CurrentUserService(IHttpContextAccessor http) : ICurrentUserService
        {
        private readonly ClaimsPrincipal? _user = http.HttpContext?.User;

        /// <summary>The authenticated user's Guid Id.</summary>
        public Guid? UserId
        {
            get
            {
                var v = _user?.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(v, out var id) ? id : null;
            }
        }

        /// <summary>Email address from the JWT.</summary>
        public string? Email => _user?.FindFirstValue(ClaimTypes.Email);

        /// <summary>
        /// Primary Identity role (e.g., "Seller", "Buyer", "Admin").
        /// If the user has multiple roles, this returns the first one in the token.
        /// </summary>
        public string? Role => _user?.FindFirstValue(ClaimTypes.Role);

        /// <summary>
        /// The company this user OWNS (Guid from "owned_company_id" claim).
        /// Non-null only for Seller role.
        /// </summary>
        public Guid? OwnedCompanyId
        {
            get
            {
                var v = _user?.FindFirstValue("owned_company_id");
                return Guid.TryParse(v, out var id) ? id : null;
            }
        }

        /// <summary>
        /// The company this user works FOR (Guid from "employer_company_id" claim).
        /// For a Seller who owns their company: OwnedCompanyId == EmployerCompanyId.
        /// </summary>
        public Guid? EmployerCompanyId
        {
            get
            {
                var v = _user?.FindFirstValue("employer_company_id");
                return Guid.TryParse(v, out var id) ? id : null;
            }
        }

        public bool IsAuthenticated => _user?.Identity?.IsAuthenticated ?? false;
    }
    }

