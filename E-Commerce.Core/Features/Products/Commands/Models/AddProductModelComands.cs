
using E_Commerce.Core.BaseResponse;
using E_Commerce.Data;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace E_Commerce.Core.Features.Products.Commands.Models
{
    public record AddProductModelComands(
        string Name, string Description, Guid CategoryId,
        int MinimumOrderQuantity, decimal BasePrice , string Currency)
        : IRequest<ApiResponse<ProductDto>>;


}

