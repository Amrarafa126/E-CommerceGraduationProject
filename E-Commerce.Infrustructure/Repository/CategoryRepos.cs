using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrustructure.Repository
{
    public class CategoryRepos : GenericRepositoryAsync<Category>, ICategoryRepos
    {
        DbSet<Category> categories;
        public CategoryRepos(AppDBContext dbContext) : base(dbContext)
        {
            categories = dbContext.Set<Category>();
        }

        public async Task<List<Category>> GetCategoryListAsync()
        {
            var category = await categories.Where(x=>x.ParentId == null).Include(x => x.CategoryChildren).ToListAsync();
            return category;
        }
    }
    }

