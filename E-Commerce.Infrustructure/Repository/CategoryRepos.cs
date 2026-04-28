using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrustructure.Repository
{
    public class CategoryRepos(AppDBContext Db) : GenericRepositoryAsync<Category>(Db), ICategoryRepos
    { 
       public async Task<IEnumerable<Category>> GetRootsWithChildrenAsync(CancellationToken ct = default)
       => await Db.categories.AsNoTracking()
           .Where(c => c.ParentCategoryId == null)
           .Include(c => c.SubCategories).ThenInclude(sc => sc.Products)
           .OrderBy(c => c.Name)
           .ToListAsync(ct);

        public Task<Category?> GetWithChildrenAsync(Guid id, CancellationToken ct = default)
            => Db.categories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories).ThenInclude(sc => sc.SubCategories)
                .Include(c => c.SubCategories).ThenInclude(sc => sc.Products)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
      
    }
 }

