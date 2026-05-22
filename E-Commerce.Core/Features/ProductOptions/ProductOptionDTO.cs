using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductOptions
{
    public record ProductOptionDto(Guid Id, string Name, int DisplayOrder,
     List<ProductOptionValueDto> Values);

    public record UpdateProductOptionDto(string Name, int DisplayOrder = 0);
    public record UpdateOptionValueDto(string Value, int DisplayOrder = 0);

    public record AddProductOptionDto(string Name, int DisplayOrder = 0, List<string>? Values = null);
    public record AddOptionValueDto(string Value, int DisplayOrder = 0);

    public record ProductOptionValueDto(Guid Id, string Value, int DisplayOrder);

}
