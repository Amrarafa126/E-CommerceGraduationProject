using E_Commerce.Core.BaseResponse;
using MediatR;

namespace E_Commerce.Core.Features.ProductOptionValues.Commands.Models
{
    public class AddOptionValueCommand : IRequest<Response<string>>
    {
        public int ProductOptionId { get; set; }
        public string Value { get; set; }
    }
}
