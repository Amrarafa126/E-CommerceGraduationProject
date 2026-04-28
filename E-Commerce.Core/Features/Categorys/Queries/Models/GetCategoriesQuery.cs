
using E_Commerce.Core.BaseResponse;
using MediatR;

namespace E_Commerce.Core.Features.Categorys.Queries.Models
{
    public record GetCategoriesQuery : IRequest<ApiResponse<List<CategoryDto>>>;

}
