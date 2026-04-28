using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductPriceTiers
{
    public record PriceTierDto(Guid Id, int MinQuantity, int? MaxQuantity, decimal UnitPrice);
    public record AddPriceTierDto(int MinQuantity, decimal UnitPrice, int? MaxQuantity = null);



}
