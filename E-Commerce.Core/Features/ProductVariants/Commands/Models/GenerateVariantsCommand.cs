using E_Commerce.Core.BaseResponse;
using MediatR;


namespace E_Commerce.Core.Features.ProductVariants.Commands.Models
{
    public class GenerateVariantsCommand : IRequest<Response<string>>
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

    }
}
