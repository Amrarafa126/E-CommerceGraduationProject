using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;

namespace E_Commerce.Service.Interfase
{
    public interface IProductOptionService
    
     {
           public Task<string> AddProductOptionAsync(ProductOption productOption);
           // Task<string> EditProductOptionAsync(int id, string name);
            //Task<string> DeleteProductOptionAsync(int id);
    }
}
