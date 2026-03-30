using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Features.ProductOptionValues.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Service.Interfase;
using E_Commerce.Service.Repostoiry;
using MediatR;

namespace E_Commerce.Core.Features.ProductOptionValues.Commands.Handlers
{
    public class ProductOptionValueHandlersCommands : ResponseHandler, IRequestHandler<AddOptionValueCommand, Response<string>>
    {
        IMapper mapper;
        IProductOptionValueService valueService;
        public ProductOptionValueHandlersCommands(IMapper mapper, IProductOptionValueService valueService)
        {
            this.mapper = mapper;
            this.valueService = valueService;

        }
        public async Task<Response<string>> Handle(AddOptionValueCommand request, CancellationToken cancellationToken)
        {

            var ProductOptionValueMapper = mapper.Map<ProductOptionValue>(request);

            var result = await valueService.AddProductOptionValueAsync(ProductOptionValueMapper);
            if (result == null)
                return UnprocessableEntity<string>();
            return Success(result);
        }
    }
}
