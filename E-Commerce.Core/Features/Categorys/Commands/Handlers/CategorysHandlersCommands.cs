using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Categorys.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Categorys.Commands.Handlers
{
    public class CategorysHandlersCommands(IUnitOfWork uow)
    : IRequestHandler<CreateCategoryCommand, ApiResponse<CategoryDto>>
    {
        public async Task<ApiResponse<CategoryDto>> Handle(CreateCategoryCommand req, CancellationToken ct)
        {
            if (req.ParentCategoryId.HasValue)
            {
                var parent = await uow.Category.GetByIdAsync(req.ParentCategoryId.Value, ct)
                    ?? throw new NotFoundException(nameof(Category), req.ParentCategoryId.Value);
            }

            var category = Category.Create(req.Name, req.Description, req.ParentCategoryId);
            await uow.Category.AddAsync(category, ct);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<CategoryDto>.Created(new CategoryDto(
                category.Id, category.Name,category.Description,
                category.ParentCategoryId, null, new()));
        }
    }
}
