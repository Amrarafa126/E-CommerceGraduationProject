
//using E_Commerce.Data.Entity;
//using E_Commerce.Infrustructure.Interfase;
//using E_Commerce.Infrustructure.Repository;
//using E_Commerce.Service.Interfase;
//using Microsoft.AspNetCore.Http.HttpResults;

//namespace E_Commerce.Service.Repostoiry
//{
//    public class ProductImageService : IProductImageService
//    {
//        IProductImageRepos ProductImageRepos;
//        public ProductImageService(IProductImageRepos productImageRepos)
//        {
//            ProductImageRepos = productImageRepos;
//        }
//        public async Task<string> AddProductImageAsync(List<ProductImage> productImage)
//        {
//            try
//            {
//                await ProductImageRepos.AddRangeAsync(productImage);
//                return ("Product image added successfully");
//            }
//            catch (Exception ex)
//            {
//                return ($"An error occurred while adding the product image: {ex.Message}");

//                // 🔐 Multi-Tenancy Security
//                //if (product.CompanyId != _currentUserService.CompanyId)
//                //    return Unauthorized<string>();
//            }
//        }

//        public async Task<string> DeleteProductImageAsync(ProductImage productImage)
//        {
//            try
//            {
//               await ProductImageRepos.DeleteAsync(productImage);
//                return ("Product image deleted successfully");
//            }
//            catch (Exception ex)
//            {
//                return ($"An error occurred while deleting the product image: {ex.Message}");
//            }

//        }

//        public Task<string> EditProductImageAsync(ProductImage productImage)
//        {
//            var EditProdctImage = ProductImageRepos.UpdateAsync(productImage);
//            if(EditProdctImage == null) return Task.FromResult("Failed Edit");
//            return Task.FromResult("Success");
//        }

//        public async Task<ProductImage> GetByIdAsync(int id)
//        {
//            return await ProductImageRepos.GetByIdAsync(id);

//        }
//    }
//}
