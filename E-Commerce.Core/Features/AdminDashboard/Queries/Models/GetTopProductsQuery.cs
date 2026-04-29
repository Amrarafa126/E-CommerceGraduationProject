using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.AdminDashboard.Queries.Models
{
    public record GetTopProductsQuery(int Limit = 10) : IRequest<ApiResponse<List<TopProductDto>>>;

}
