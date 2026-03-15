using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InfrustructureBases;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface ICategoryRepos : IGenericRepositoryAsync<Category>
    {
        public  Task<List<Category>> GetCategoryListAsync();
      

    }
}
