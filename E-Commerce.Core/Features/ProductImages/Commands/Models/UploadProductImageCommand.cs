using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Core.BaseResponse;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace E_Commerce.Core.Features.ProductImages.Commands.Models
{
    public record UploadProductImageCommand(
        Guid ProductId, IFormFile File,
        string? AltText = null, int DisplayOrder = 0)
        : IRequest<ApiResponse<ProductImageDto>>;
}
