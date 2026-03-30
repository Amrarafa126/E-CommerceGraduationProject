
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Interfase;
using E_Commerce.Infrustructure.Repository;
using E_Commerce.Service.Interfase;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace E_Commerce.Service.Repostoiry
{
    public class ProductOptionService : IProductOptionService
    {
        IProductOptionsRepos ProductOptionsRepos;
        public ProductOptionService(IProductOptionsRepos productOptionsRepos)
        {
            ProductOptionsRepos = productOptionsRepos;
        }
        public async Task<string> AddProductOptionAsync(ProductOption productOption)
        {
            var ProductOptionCheak = ProductOptionsRepos.GetTableNoTracking().Where(c => c.Name == productOption.Name).FirstOrDefault();
            if (ProductOptionCheak != null) return "Exist";


            var AddProductOption = await ProductOptionsRepos.AddAsync(productOption);

            if (AddProductOption == null) return "Failed Add";
            return "Success";
        }
    }
}
