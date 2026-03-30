using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace E_Commerce.Core.Mapping.ProductImage
{
    public partial class ProductImagesProfile : Profile
    {
            public ProductImagesProfile()
            {
                AddProductImagesCommandMapping();
                EditProductImageMapping();
        }
    }
}
