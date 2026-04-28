using AutoMapper;
using E_Commerce.Core.Features.ProductReview;
using E_Commerce.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Mapping.ProuductReview
{
    public partial class ReviewProfile
    {
        public void AddReviewMapping()
        {
            CreateMap<ProductReview, ProductReviewDto>()
           .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : ""))
           .ForMember(d => d.BuyerName, o => o.MapFrom(s => s.Buyer != null ? s.Buyer.FullName : ""))
           .ForMember(d => d.ImageUrls, o => o.MapFrom(s =>
               s.Images.Select(i => i.Url).ToList()));
        }

    }
}
