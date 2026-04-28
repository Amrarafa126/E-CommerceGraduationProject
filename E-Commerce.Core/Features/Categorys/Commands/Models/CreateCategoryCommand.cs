
using E_Commerce.Core.BaseResponse;
using MediatR;

namespace E_Commerce.Core.Features.Categorys.Commands.Models
{

    public record CreateCategoryCommand(
        string Name, string? Description = null,
        Guid? ParentCategoryId = null)
        : IRequest<ApiResponse<CategoryDto>>;
}
