
//using AutoMapper;
//using E_Commerce.Core.BaseResponse;
//using E_Commerce.Service.Interfase;
//using E_Commerce.Data.Entity;
//using MediatR;
//using E_Commerce.Core.Features.Categorys.Commands.Models;

//namespace E_Commerce.Core.Features.Categorys.Commands.Handlers
//{
//    public class CategoryHandlersCommand : ResponseHandler , IRequestHandler<AddCategoryCommand, Response<string>>
//                                                           , IRequestHandler<EditCategoryCommand, Response<string>>
//                                                           , IRequestHandler<DeleteCategoryCommand, Response<string>>
//    {
//        IMapper mapper;
//        ICategoryService CategoryService; 
//        public CategoryHandlersCommand(IMapper mapper , ICategoryService categoryService)
//        {
//            this.mapper = mapper;
//            this.CategoryService = categoryService;

//        }

//        public async Task<Response<string>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
//        {
//            var categoryMapper = mapper.Map<Category>(request);

//            var result = await CategoryService.AddCategoryAsync(categoryMapper);
//            if (result == null)
//                return UnprocessableEntity<string>();
//            return Success(result);

//        }

//        public async Task<Response<string>> Handle(EditCategoryCommand request, CancellationToken cancellationToken)
//        {
//            var Category = await CategoryService.GetCategoryByIdAsync(request.Id);
//            if (Category == null) return NotFound<string>();
//            var CategoryMapper = mapper.Map(request, Category);
//            var result = await CategoryService.EditCategoryAsync(CategoryMapper);
//            if (result == null) return BadRequest<string>();
//            return Success("Edit Sussessfully");
//        }

//        public async Task<Response<string>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
//        {
           
//            var Category = await CategoryService.GetCategoryByIdAsync(request.Id);
//            if (Category == null) return NotFound<string>();
//            var result = await CategoryService.DeleteCategoryAsync(Category);
//            if (result == null) return BadRequest<string>();
//            return Success("Delete Sussessfully");
//        }
//    }
//}
