using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_Commerce.Core.Features.Categorys.Commands.Models;
using E_Commerce.Core.Features.ProductImages.Commands.Models;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Mapping.ProductImage
{
    public partial class ProductImagesProfile 
    {
        public void EditProductImageMapping()
        {
            CreateMap<UpdateProductImageCommand, Data.Entity.ProductImage>();
           
        }

    }
}
