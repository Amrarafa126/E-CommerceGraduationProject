using E_Commerce.Core.Features.ProductImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Products
{

    public record ProductDto(
        Guid Id,
        string Name,
        string Description,
        int MinimumOrderQuantity,
        string Status,
        Guid CompanyId,
        string? CompanyName,
        Guid CategoryId,
        string? CategoryName,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}

public record CreateProductDto(string Name, string Description, Guid CategoryId,
    int MinimumOrderQuantity, decimal BasePrice , string currency);