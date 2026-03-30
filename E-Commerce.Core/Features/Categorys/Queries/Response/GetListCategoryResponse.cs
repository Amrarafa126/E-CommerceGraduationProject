using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Features.Categorys.Queries.Response
{
    public class GetListCategoryResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<CategoryResponse>? CategoryList { get; set; }
        public class CategoryResponse
        {
            public int Id { get; set; }
            public string? NameCategory { get; set; }
        }


    }
}
