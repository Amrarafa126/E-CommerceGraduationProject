using E_Commerce.Core.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.RFQ.Queries.Models
{
    public record GetSellerRfqsQuery(int Page = 1, int PageSize = 20, int? Status = null)
      : IRequest<ApiResponse<PaginatedResult<RfqRequestDto>>>;
}
