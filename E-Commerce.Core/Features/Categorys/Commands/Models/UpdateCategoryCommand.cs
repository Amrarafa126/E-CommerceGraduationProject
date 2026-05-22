using MediatR;

namespace E_Commerce.Core.Features.Categorys.Commands.Models
{
    public record UpdateCategoryCommand(
        Guid CategoryId,
        string Name,
        string? Description = null,
        Guid? ParentCategoryId = null)
        : IRequest<ApiResponse<CategoryDto>>;
}
