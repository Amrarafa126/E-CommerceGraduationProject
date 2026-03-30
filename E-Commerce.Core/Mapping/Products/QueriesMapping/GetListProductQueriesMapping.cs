using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_Commerce.Core.Features.Products.Queries.Response;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Mapping.Products
{
    public partial class ProductsProfile 
    {
        public void GetListProductMapping()
        {
            CreateMap<Product, GetListProductResponse>()
                .ForMember(dest => dest.Image,
                         opt => opt.MapFrom(src => src.Images!.Where(x => x.IsMain).Select(x => x.ImageUrl).FirstOrDefault()
                         ?? "/images/default-product.png"))
                .ForMember(dest => dest.MOQ, opt => opt.MapFrom(src => src.MinimumOrderQuantity));
        }

    }
}
