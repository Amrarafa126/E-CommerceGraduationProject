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
    public class AddProductImagesCommand : IRequest<Response<string>>
    {
        public int ProductId { get; set; }
        public List<IFormFile> Images { get; set; } = new();
    }
}
