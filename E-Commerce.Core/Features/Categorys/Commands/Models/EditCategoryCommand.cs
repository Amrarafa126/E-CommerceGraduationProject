using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Core.BaseResponse;
using MediatR;

namespace E_Commerce.Core.Features.Categorys.Commands.Models
{
    public class EditCategoryCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
        public string? NameCategory { get; set; }
        public int? ParentId { get; set; }
    }
 }

