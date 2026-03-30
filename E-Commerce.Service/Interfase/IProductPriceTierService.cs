using E_Commerce.Data.Entity;

namespace E_Commerce.Service.Interfase
{
    public interface IProductPriceTierService
    {
        public Task<string> AddPriceTierAsync(ProductPriceTier productPriceTier);
    }
}
