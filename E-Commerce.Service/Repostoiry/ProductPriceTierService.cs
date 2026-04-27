
//using E_Commerce.Data.Entity;
//using E_Commerce.Infrustructure.Interfase;
//using E_Commerce.Service.Interfase;

//namespace E_Commerce.Service.Repostoiry
//{
//    public class ProductPriceTierService : IProductPriceTierService
//    {
//        IProductPriceTierRepos ProductPriceTier;
//        public ProductPriceTierService(IProductPriceTierRepos ProductPriceTier)
//        {
//            this.ProductPriceTier = ProductPriceTier;

//        }
//        public async Task<string> AddPriceTierAsync(ProductPriceTier productPrice)
//        {

//            var result = await ProductPriceTier.AddAsync(productPrice);
//            if (result!= null)
//            {
//                return "Price tier added successfully.";
//            }
//            else
//            {
//                return "Failed to add price tier.";
//            }


//        }
//    }
//}
