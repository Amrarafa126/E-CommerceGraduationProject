using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Interfase;
using E_Commerce.Service.Interfase;

namespace E_Commerce.Service.Repostoiry
{
    public class CategoryService :ICategoryService
    {
        ICategoryRepos CategoryRepos;
        public CategoryService(ICategoryRepos CategoryRepos) 
        {
            this.CategoryRepos = CategoryRepos;
        
        }
        public async Task<List<Category>> GetCategoryListAsync()
        {
            return await CategoryRepos.GetCategoryListAsync();
        }

    }
}
