
using AutoMapper;

namespace E_Commerce.Core.Mapping.Products
{
    public partial class ProductsProfile :Profile
    {
        public ProductsProfile()
        {
            AddProductMapping();
        }
    }
}
