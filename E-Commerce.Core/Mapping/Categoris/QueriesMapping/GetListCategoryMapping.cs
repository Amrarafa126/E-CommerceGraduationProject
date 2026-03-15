using E_Commerce.Data.Entity;
using E_Commerce.Core.Features.Category.Queries.Response;
using Microsoft.Extensions.Caching.Memory;

namespace E_Commerce.Core.Mapping.Categoris
{
    public partial class CategoryProfile
    {
        public void GetCategoryListMapping()
        {
           // CreateMap<Category, GetListCategoryResponse>().ForMember(x => x.Parent, opt => opt.MapFrom(src => src.Parent));

        }
    }
}
