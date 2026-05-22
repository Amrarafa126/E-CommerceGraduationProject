using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Categorys.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Categorys.Commands.Handlers
{
    public class CategorysHandlersCommands(IUnitOfWork uow, ICurrentUserService cu)
    : IRequestHandler<CreateCategoryCommand, ApiResponse<CategoryDto>>,
      IRequestHandler<UpdateCategoryCommand, ApiResponse<CategoryDto>>,
      IRequestHandler<DeleteCategoryCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<CategoryDto>> Handle(CreateCategoryCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();
            if (cu.Role != "Admin") throw new ForbiddenException("Admin only.");
            if (req.ParentCategoryId.HasValue)
            {
                var parent = await uow.Category.GetByIdAsync(req.ParentCategoryId.Value, ct)
                    ?? throw new NotFoundException(nameof(Category), req.ParentCategoryId.Value);
            }

            var category = Category.Create(req.Name, req.Description, req.ParentCategoryId);
            await uow.Category.AddAsync(category, ct);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<CategoryDto>.Created(new CategoryDto(
                category.Id, category.Name, category.Description,
                category.ParentCategoryId, null, new()));
        }

        public async Task<ApiResponse<CategoryDto>> Handle(UpdateCategoryCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();
            if (cu.Role != "Admin") throw new ForbiddenException("Admin only.");
            var category = await uow.Category.GetByIdAsync(req.CategoryId, ct)
                ?? throw new NotFoundException(nameof(Category), req.CategoryId);

            if (req.ParentCategoryId.HasValue && req.ParentCategoryId != category.ParentCategoryId)
            {
                var parent = await uow.Category.GetByIdAsync(req.ParentCategoryId.Value, ct)
                    ?? throw new NotFoundException(nameof(Category), req.ParentCategoryId.Value);
            }

            category.Update(req.Name, req.Description, req.ParentCategoryId);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<CategoryDto>.Ok(new CategoryDto(
                category.Id, category.Name, category.Description,
                category.ParentCategoryId, category.ParentCategory?.Name, new()));
        }

        public async Task<ApiResponse<object>> Handle(DeleteCategoryCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();
            if (cu.Role != "Admin") throw new ForbiddenException("Admin only.");
            var category = await uow.Category.GetByIdAsync(req.CategoryId, ct)
                ?? throw new NotFoundException(nameof(Category), req.CategoryId);

            category.SoftDelete();
            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok("Category deleted successfully.");
        }
    }
}
