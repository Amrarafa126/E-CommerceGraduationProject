using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductVariantsValue
{
    public record VariantOptionValueDto(Guid Id, string Value, string? DisplayLabel, string OptionName);

}
