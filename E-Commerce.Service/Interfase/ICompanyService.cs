using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;

namespace E_Commerce.Service.Interfase
{
    public interface ICompanyService
    {
            public Task<List<Company>> GetCompanyListAsync();
            public Task<string> AddCategoryAsync(Company company);
    }
}
