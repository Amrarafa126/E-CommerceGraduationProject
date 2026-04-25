
//using AutoMapper;
//using E_Commerce.Core.BaseResponse;
//using E_Commerce.Core.Features.Categorys.Commands.Models;
//using E_Commerce.Core.Features.ProductImages.Commands.Models;
//using E_Commerce.Data.Entity;
//using E_Commerce.Service.Interfase;
//using E_Commerce.Service.Repostoiry;
//using MediatR;
//using Microsoft.AspNetCore.Http;

//namespace E_Commerce.Core.Features.ProductImages.Commands.Handlers
//{
//    public class ProductImageHandlersCommands : ResponseHandler, IRequestHandler<AddProductImagesCommand, Response<string>>
//                                                               , IRequestHandler<DeleteProductImageCommand, Response<string>>
//                                                               , IRequestHandler<UpdateProductImageCommand, Response<string>>
//    {
//        private readonly IFileService _fileService;
//        IProductService productService;
//        IProductImageService ProductImageService;
//        IMapper mapper;

//        public ProductImageHandlersCommands(IFileService fileService,
//                                            IProductService productService,
//                                            IProductImageService productImageService,
//                                            IMapper mapper)
//        {
//            _fileService = fileService;
//            this.productService = productService;
//            ProductImageService = productImageService;
//            this.mapper = mapper;
//        }
//        public async Task<Response<string>> Handle(AddProductImagesCommand request, CancellationToken cancellationToken)
//        {

//            if (request.Images == null || !request.Images.Any())
//                return BadRequest<string>("No images uploaded");

//            var product = await productService.GetByIdAsync(request.ProductId);

//            if (product == null)
//                return NotFound<string>("Product not found");

//            var imageUrls = await _fileService.UploadProductImages(
//                "images/products",
//                request.Images
//            );


//            var images = imageUrls.Select(url => new ProductImage
//            {
//                ImageUrl = url,
//                ProductId = product.Id,
//                IsMain = false

//            }).ToList();

//            if (!(product.Images?.Any() ?? false) && images.Any())
//            {
//                images.First().IsMain = true;
//            }

//            await ProductImageService.AddProductImageAsync(images);

//            return Success("Images added successfully");
//        }

//        public async Task<Response<string>> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
//        {
//            var image = await ProductImageService.GetByIdAsync(request.Id);
//            if (image == null)
//                return NotFound<string>("Image not found");
//            var result = await ProductImageService.DeleteProductImageAsync(image);
//            if (result == null)
//                return BadRequest<string>("Failed to delete image");
//            return Success("Image deleted successfully");
//        }

//        public async Task<Response<string>> Handle(UpdateProductImageCommand request, CancellationToken cancellationToken)
//        {
//            var image = await ProductImageService.GetByIdAsync(request.ImageId);
//            if (image == null)
//                return NotFound<string>("Image not found");
//            // var ProductImageMapper = mapper.Map(request, image);
//            // var result = await ProductImageService.EditProductImageAsync(ProductImageMapper);
//            var urls = await _fileService.UploadProductImages(
//            "images/products",
//          new List<IFormFile> { request.NewImage! });

//            image.ImageUrl = urls.First();

//            await ProductImageService.EditProductImageAsync(image);

//            return Success("Image updated successfully");
//        }
//    }
//}


//// 🔥 تحويل لـ Entity
////var images = imageUrls.Select(url => new ProductImage
////{
////    ImageUrl = imageUrls,
////    ProductId = request.ProductId,
////    IsMain = false
////}).ToList();

////// 🔐 Multi-Tenancy
////if (product.CompanyId != _currentUserService.CompanyId)
////    return Unauthorized<string>();