using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Core.Features.Categorys.Commands.Models;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Mapping.Categoris
{
    public partial class CategoryProfile
    {
        public void EditCategoryMapping()
        {
            CreateMap<EditCategoryCommand, Category>()
                .ForMember(x => x.ParentId, opt => opt.MapFrom(src => src.ParentId));
        }
    }

}
