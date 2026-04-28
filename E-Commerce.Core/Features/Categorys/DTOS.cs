using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Categorys
{
    public record CategoryDto(Guid Id, string Name, string? Description, Guid? ParentCategoryId, string? ParentCategoryName, List<CategoryChildDto> Children);
    public record CategoryChildDto(Guid Id, string Name,  string? Description, int ProductCount);
    public record CreateCategoryDto(string Name, string? Description = null, Guid? ParentCategoryId = null);
    public record UpdateCategoryDto(string Name, string? Description = null);
}
