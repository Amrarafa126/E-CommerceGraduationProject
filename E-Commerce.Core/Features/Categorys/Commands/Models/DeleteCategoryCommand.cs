using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using E_Commerce.Core.BaseResponse;
using MediatR;

namespace E_Commerce.Core.Features.Categorys.Commands.Models
{
    public class DeleteCategoryCommand :IRequest<BaseResponse.Response<string>>
    {
        public int Id { get; set; }
        public DeleteCategoryCommand(int id)
        {
            Id = id;
        }
    }
}
