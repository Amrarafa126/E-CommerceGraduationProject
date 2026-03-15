using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Features.Category.Queries.Response;
using MediatR;

namespace E_Commerce.Core.Features.Category.Queries.Models
{
    public class GetListCategoryQueries : IRequest<Response<List<GetListCategoryResponse>>>
    {
    }
}
