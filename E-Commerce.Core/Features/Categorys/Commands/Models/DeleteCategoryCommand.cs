using MediatR;

namespace E_Commerce.Core.Features.Categorys.Commands.Models
{
    public record DeleteCategoryCommand(Guid CategoryId)
        : IRequest<ApiResponse<object>>;
}
