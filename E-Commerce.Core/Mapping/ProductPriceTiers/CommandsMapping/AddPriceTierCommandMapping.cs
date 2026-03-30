using E_Commerce.Core.Features.ProductPriceTiers.Commands.Models;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Mapping.ProductPriceTiers
{
    public partial class ProductPriceTierProfile
    {
      public void AddPriceTierCommandMapping()
      {
            CreateMap<AddPriceTierCommand, ProductPriceTier>();
      }
    }
}
