using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Core.BaseResponse;
using MediatR;

namespace E_Commerce.Core.Features.ProductImages.Commands.Models
{
    public class DeleteProductImageCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
        public DeleteProductImageCommand(int id)
        {
            Id = id;

        }
    }
}
