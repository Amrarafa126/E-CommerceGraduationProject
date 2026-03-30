
using E_Commerce.Core.BaseResponse;
using MediatR;

namespace E_Commerce.Core.Features.Categorys.Commands.Models
{
    public class AddCategoryCommand : IRequest<Response<string>>
    {
        public string? NameCategory { get; set; }
        public int? ParentId { get; set; }
    }
}
