
using E_Commerce.Core.BaseResponse;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace E_Commerce.Core.Features.Products.Commands.Models
{
    public class AddProductModelComands : IRequest<Response<string>>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        public int MinimumOrderQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }


}

