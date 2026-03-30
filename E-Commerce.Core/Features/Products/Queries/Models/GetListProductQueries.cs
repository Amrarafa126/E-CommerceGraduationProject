using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Features.Products.Queries.Response;
using MediatR;

namespace E_Commerce.Core.Features.Products.Queries.Models
{
    public class GetListProductQueries : IRequest<Response<List<GetListProductResponse>>>
    {
    }
}
