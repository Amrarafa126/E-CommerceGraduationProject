
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Features.Categorys.Queries.Response;
using MediatR;

namespace E_Commerce.Core.Features.Categorys.Queries.Models
{
    public class GetListCategoryQueries : IRequest<Response<List<GetListCategoryResponse>>>
    {
        //////////
    }
}
