using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Companies.Queries.Models
{
    public record GetCompanyByIdQuery(Guid CompanyId) : IRequest<ApiResponse<CompanyDto>>;

}
