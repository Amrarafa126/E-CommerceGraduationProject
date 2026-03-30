using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;

namespace E_Commerce.Service.Interfase
{
    public interface ICategoryService
    {
        public Task<List<Category>> GetCategoryListAsync();
        public Task<string> AddCategoryAsync(Category category);
        public Task<Category> GetCategoryByIdAsync(int id);
        public Task<string> EditCategoryAsync(Category category);

        public Task<string> DeleteCategoryAsync(Category category);
      
        
    }
}
