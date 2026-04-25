using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductOptionValues
{
    public record ProductOptionValueDto(Guid Id, string Value, string? DisplayLabel, int DisplayOrder);

}
