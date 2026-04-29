using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductImages.Commands.Models
{
    public record UpdateProductImageCommand(
     Guid ProductId,
     Guid ImageId,
     IFormFile? File = null,
     string? AltText = null,
     int? DisplayOrder = null)
     : IRequest<ApiResponse<ProductImageDto>>;
}
