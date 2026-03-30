using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Interfase;
using E_Commerce.Infrustructure.Repository;
using E_Commerce.Service.Interfase;

namespace E_Commerce.Service.Repostoiry
{
    public class ProductOptionValueService : IProductOptionValueService
    {
        IProductOptionValuesRepos ProductOptionsRepos;
        public ProductOptionValueService(IProductOptionValuesRepos ProductOptionsRepos)
        {
            this.ProductOptionsRepos = ProductOptionsRepos;
        }
        public async Task<string> AddProductOptionValueAsync(ProductOptionValue productOptionValue)
        {
            var ProductOptionValueCheak =  ProductOptionsRepos.GetTableNoTracking().Where(c => c.Value == productOptionValue.Value).FirstOrDefault();
            if (ProductOptionValueCheak != null) return "Exist";


            var AddProductOptionValue = await ProductOptionsRepos.AddAsync(productOptionValue);

            if (AddProductOptionValue == null) return "Failed Add";
            return "Success";
        }
    }
}
