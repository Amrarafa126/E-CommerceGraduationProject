using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Core.BaseResponse;
using MediatR;

namespace E_Commerce.Core.Features.Companies.Commands.Models
{
    public class AddCompanyCommands : IRequest<Response<string>>
    {
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? CoverUrl { get; set; }
    }
}
