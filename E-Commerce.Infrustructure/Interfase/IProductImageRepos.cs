
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InfrustructureBases;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface IProductImageRepos : IGenericRepositoryAsync<ProductImage>
    {
        public Task<List<ProductImage>> AddListAsync(List<ProductImage> entity);
       

    }
}
