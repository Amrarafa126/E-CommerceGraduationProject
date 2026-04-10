using E_Commerce.Core.Features.ApplicationUser.Queries.Response;
using E_Commerce.Core.Wrappers;
using MediatR;

namespace E_Commerce.Core.Features.ApplicationUser.Queries.Models
{
    public class GetUserPaginationQuery : IRequest<PaginatedResult<GetUserPaginationReponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
