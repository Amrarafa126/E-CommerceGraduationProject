using E_Commerce.Core.Features.Categorys.Queries.Response;
using E_Commerce.Data.Entity;
using Microsoft.Extensions.Caching.Memory;
using static E_Commerce.Core.Features.Categorys.Queries.Response.GetListCategoryResponse;

namespace E_Commerce.Core.Mapping.Categoris
{
    public partial class CategoryProfile
    {
        public void GetCategoryListMapping()
        {
            //CreateMap<Category, GetListCategoryResponse>()
            //   .ForMember(d => d.Id, o => o.MapFrom(src => src.Id))
            //   .ForMember(d => d.Name, o => o.MapFrom(src => src.NameCategory))
            //   .ForMember(d => d.CategoryList, o => o.MapFrom(src => src.CategoryChildren));

            //           CreateMap<Category, CategoryResponse>()
            //    .ForMember(d => d.Id, o => o.MapFrom(src => src.Id))
            //    .ForMember(d => d.NameCategory, o => o.MapFrom(src => src.NameCategory));
          
        }
    }
}
