
using MediatR;

namespace E_Commerce.Core.Features.ProductImages.Commands.Models
{
    public record DeleteProductImageCommand(Guid ProductId, Guid ImageId)
     : IRequest<ApiResponse<object>>;
}
