
using System.Text.Json.Serialization;
using E_Commerce.Core.BaseResponse;
using MediatR;

namespace E_Commerce.Core.Features.ProductOptions.Commands.Models
{
    public class AddProductOptionCommand : IRequest<Response<string>>
    {
        [JsonIgnore]
        public int ProductId { get; set; }
        public string? Name { get; set; }
    }
}
