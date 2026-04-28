using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Core.BaseResponse;
using MediatR;

namespace E_Commerce.Core.Features.ProductImages.Commands.Models
{
    public record DeleteProductImageCommand(Guid ProductId, Guid ImageId)
     : IRequest<ApiResponse<object>>;
}
