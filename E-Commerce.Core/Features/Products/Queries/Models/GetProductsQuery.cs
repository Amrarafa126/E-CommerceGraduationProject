using E_Commerce.Core.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Products.Queries.Models
{
    public record GetProductsQuery(
     Guid? CategoryId, Guid? CompanyId, string? Search,
     decimal? MinPrice, decimal? MaxPrice, int Page = 1, int PageSize = 20)
     : IRequest<ApiResponse<PaginatedResult<ProductSummaryDto>>>;
}
