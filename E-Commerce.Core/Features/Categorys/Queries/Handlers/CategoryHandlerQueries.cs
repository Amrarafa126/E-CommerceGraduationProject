using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Categorys.Queries.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using MediatR;

namespace E_Commerce.Core.Features.Categorys.Queries.Handlers
{
    public class CategoryHandlerQueries(IUnitOfWork uow)
    : IRequestHandler<GetCategoriesQuery, ApiResponse<List<CategoryDto>>>,
         IRequestHandler<GetCategoryByIdQuery, ApiResponse<CategoryDto>>
    {
        public async Task<ApiResponse<List<CategoryDto>>> Handle(GetCategoriesQuery req, CancellationToken ct)
        {
            var roots = await uow.Category.GetRootsWithChildrenAsync(ct);
            var dtos = roots.Select(c => new CategoryDto(
                c.Id, c.Name, c.Description,
                c.ParentCategoryId, null,
                c.SubCategories.Select(sc => new CategoryChildDto(
                    sc.Id, sc.Name,  sc.Description, 
                    sc.Products.Count)).ToList())).ToList();
            return ApiResponse<List<CategoryDto>>.Ok(dtos);
        }

        public async Task<ApiResponse<CategoryDto>> Handle(GetCategoryByIdQuery req, CancellationToken ct)
        {
            var cat = await uow.Category.GetWithChildrenAsync(req.CategoryId, ct)
                ?? throw new NotFoundException(nameof(Category), req.CategoryId);

            return ApiResponse<CategoryDto>.Ok(new CategoryDto(
                cat.Id, cat.Name,  cat.Description,
                cat.ParentCategoryId,
                cat.ParentCategory?.Name,
                cat.SubCategories.Select(sc => new CategoryChildDto(
                    sc.Id, sc.Name, sc.Description, 
                    sc.Products.Count)).ToList()));
        }

    }
}

