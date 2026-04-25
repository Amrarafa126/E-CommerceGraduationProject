
using AutoMapper;
using E_Commerce.Core.Features.Categorys.Commands.Models;
using E_Commerce.Core.Features.Products;
using E_Commerce.Core.Features.Products.Commands.Models;
using E_Commerce.Data.Entity;


namespace E_Commerce.Core.Mapping.Products
{
    public partial class ProductsProfile
    {
            
        public void AddProductMapping()
        {
            CreateMap<Product, ProductDto>();
           
        }



    }
}
