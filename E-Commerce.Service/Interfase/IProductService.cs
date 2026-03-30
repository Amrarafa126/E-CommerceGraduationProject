
using E_Commerce.Data.Entity;
using Microsoft.AspNetCore.Http;

namespace E_Commerce.Service.Interfase
{
    public interface IProductService
    {
        public Task<List<Product>> GetProductListAsync();
        public Task<string> AddProductAsync(Product product);
        public Task<string> EditProductAsync(Product product);
        public Task<string> DeleteProductAsync(Product product);
        public Task<Product> GetByIdAsync(int id);

    }
}
