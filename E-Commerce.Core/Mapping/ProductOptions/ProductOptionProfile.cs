
using AutoMapper;

namespace E_Commerce.Core.Mapping.ProductOptions
{
    public partial class ProductOptionProfile :Profile
    {
        public ProductOptionProfile()
        {
            AddProductOptionCommandMapping();
        }
    }
}
