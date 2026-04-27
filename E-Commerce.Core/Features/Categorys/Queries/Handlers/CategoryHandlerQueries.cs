//using AutoMapper;
//using E_Commerce.Core.BaseResponse;
//using E_Commerce.Core.Features.Categorys.Queries.Models;
//using E_Commerce.Core.Features.Categorys.Queries.Response;
//using E_Commerce.Service.Interfase;
//using MediatR;

//namespace E_Commerce.Core.Features.Categorys.Queries.Handlers
//{
//    public class CategoryHandlerQueries : ResponseHandler ,IRequestHandler<GetListCategoryQueries, Response<List<GetListCategoryResponse>>>
//    {
//        ICategoryService CategoryService;
//        IMapper mapper;
//        public CategoryHandlerQueries(IMapper mapper , ICategoryService CategoryService)
//        {
//            this.mapper = mapper;
//            this.CategoryService = CategoryService;

//        }
//        public async Task<Response<List<GetListCategoryResponse>>> Handle(GetListCategoryQueries request, CancellationToken cancellationToken)
//        {
//            var CategoryList = await CategoryService.GetCategoryListAsync();
//            var categorylistMapper = mapper.Map<List<GetListCategoryResponse>>(CategoryList);
//            var res = Success(categorylistMapper);
//            res.Meta = new { Count = categorylistMapper.Count() };
//            return res;
//        }
//    }
//}

