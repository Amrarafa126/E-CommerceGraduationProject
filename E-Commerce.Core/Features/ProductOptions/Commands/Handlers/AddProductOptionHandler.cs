using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Features.ProductOptions.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Service.Interfase;
using E_Commerce.Service.Repostoiry;
using MediatR;

namespace E_Commerce.Core.Features.ProductOptions.Commands.Handlers
{
    public class AddProductOptionHandler : ResponseHandler, IRequestHandler<AddProductOptionCommand, Response<string>>
    {
        IMapper mapper;
        IProductOptionService ProductOptionService;
        public AddProductOptionHandler(IMapper mapper , IProductOptionService productOptionService)
        {
            this.mapper = mapper;
            this.ProductOptionService = productOptionService;
        }
        public async Task<Response<string>> Handle(AddProductOptionCommand request, CancellationToken cancellationToken)
        {
            var ProductOptionMapper = mapper.Map<ProductOption>(request);

            var result = await ProductOptionService.AddProductOptionAsync(ProductOptionMapper);
            if (result == null)
                return UnprocessableEntity<string>();
            return Success(result);
        }
    }
}
