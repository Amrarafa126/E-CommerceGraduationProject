using E_Commerce.Core.BaseResponse;
using MediatR;

namespace E_Commerce.Core.Features.ProductPriceTiers.Commands.Models
{
    public class AddPriceTierCommand : IRequest<Response<string>>
    {
        public int ProductId { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public decimal Price { get; set; }
    }
}
