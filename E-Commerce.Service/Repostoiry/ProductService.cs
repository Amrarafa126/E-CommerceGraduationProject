using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Interfase;
using E_Commerce.Infrustructure.Repository;
using E_Commerce.Service.Interfase;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Service.Repostoiry
{
    public class ProductService : IProductService
    {
        IProductRepos ProductRepos;
        IFileService _fileService;
        IHttpContextAccessor _httpContextAccessor;
        public ProductService(IProductRepos ProductRepos , IFileService _fileService , IHttpContextAccessor _httpContextAccessor)
        {
            this.ProductRepos = ProductRepos;
            this._fileService = _fileService;
            this._httpContextAccessor = _httpContextAccessor;
        }

        public async Task<string> AddProductAsync(Product product)
        { 
        //var context = _httpContextAccessor.HttpContext.Request;
        //    var baseUrl = context.Scheme + "://" + context.Host;
        //    var imageUrl = await _fileService.UploadProductImages($"images/products/", filesList);
       
        //    product.Images = imageUrl.Select(url => new ProductImage { ImageUrl = baseUrl + url }).ToList();

           
            //if (productVariants != null && productVariants.Any())
            //{
            //    product.productVariants = productVariants.Select(v => new ProductVariant
            //    {
            //        VariantValues = v.VariantValues,
            //        Price = v.Price,
            //        SKU = v.SKU,
            //        StockQuantity = v.StockQuantity,
            //    }).ToList();
            //}
            //if (productPriceTier != null && productPriceTier.Any())
            //{
            //    product.PriceTiers = productPriceTier.Select(p => new ProductPriceTier
            //    {
            //        MinQuantity = p.MinQuantity,
            //        MaxQuantity = p.MaxQuantity,
            //        Price = p.Price
            //    }).ToList();
            //}

            try
            {
                await ProductRepos.AddAsync(product);
                return "Success";
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding product", ex);
            }

        }

        public Task<string> DeleteProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<string> EditProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetByIdAsync(int id)
        {
            return ProductRepos.GetByIdAsync(id);
        }

        public async Task<List<Product>> GetProductListAsync()
        {
            return await ProductRepos.GetProductListAsync();
        }
    }
}
