using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;

namespace E_Commerce.Service.Interfase
{
    public interface IProductImageService
    {
        public Task<string> AddProductImageAsync(List<ProductImage> productImage);
        public Task<string> DeleteProductImageAsync(ProductImage productImage);
        public Task<ProductImage> GetByIdAsync(int id);
        public Task<string> EditProductImageAsync(ProductImage productImage);
    }
}
