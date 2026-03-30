using System;
using System.Collections.Generic;

using AutoMapper;
using E_Commerce.Core.Features.ProductImages.Commands.Models;
using E_Commerce.Data.Entity;
namespace E_Commerce.Core.Mapping.ProductImage
{
    public partial class ProductImagesProfile 
    {
        public void AddProductImagesCommandMapping()
        {
            CreateMap<AddProductImagesCommand, Data.Entity.ProductImage>()
                .ForMember(dest => dest.IsMain, opt => opt.MapFrom(src => false))
                .ForMember(d => d.ImageUrl, o => o.Ignore());


        }
    }
}