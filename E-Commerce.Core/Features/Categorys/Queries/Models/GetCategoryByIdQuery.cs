using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Categorys.Queries.Models
{
    public record GetCategoryByIdQuery(Guid CategoryId) : IRequest<ApiResponse<CategoryDto>>;

}
