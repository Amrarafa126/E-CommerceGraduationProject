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
    }
}
