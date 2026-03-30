
using AutoMapper;
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Features.Products.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Products.Commands.Handlers
{
    public class ProductHandlerCommand : ResponseHandler, IRequestHandler<AddProductModelComands, Response<string>>
    {
        IProductService ProductService;
        IMapper mapper;
        IFileService _fileService;
        public ProductHandlerCommand(IMapper mapper, IProductService productService, IFileService fileService)
        {
            this.mapper = mapper;
            ProductService = productService;
            _fileService = fileService;
        }

        public async Task<Response<string>> Handle(AddProductModelComands request, CancellationToken cancellationToken)
        {
           

            var ProductMapper = mapper.Map<Product>(request);

            var result = await ProductService.AddProductAsync(ProductMapper);
            if (result == null) return UnprocessableEntity<string>();
            return Success(result);

        }
    }
}
    

