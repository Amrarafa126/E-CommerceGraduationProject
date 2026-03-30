using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Features.Categorys.Queries.Response;
using E_Commerce.Core.Features.Products.Queries.Models;
using E_Commerce.Core.Features.Products.Queries.Response;
using E_Commerce.Service.Interfase;
using E_Commerce.Service.Repostoiry;
using MediatR;

namespace E_Commerce.Core.Features.Products.Queries.Handlers
{
    public class ProductHandlersQueries : ResponseHandler, IRequestHandler<GetListProductQueries, Response<List<GetListProductResponse>>>
    {
        IProductService productService;
        IMapper mapper;
        public ProductHandlersQueries(IProductService productService , IMapper mapper)
        {
            this.productService = productService;
            this.mapper = mapper;
        }
        public async Task<Response<List<GetListProductResponse>>> Handle(GetListProductQueries request, CancellationToken cancellationToken)
        {
            var ProductList = await productService.GetProductListAsync();
            var ProductListMapper = mapper.Map<List<GetListProductResponse>>(ProductList);
            var res = Success(ProductListMapper);
            res.Meta = new { Count = ProductListMapper.Count() };
            return res;
        }
    }
}
