using E_Commerce.Core.Features.ProductOptionValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductOptions
{
    public record ProductOptionDto(Guid Id, string Name, int DisplayOrder,
     List<ProductOptionValueDto> Values);

    public record AddProductOptionDto(string Name, int DisplayOrder = 0, List<string>? Values = null);

}
