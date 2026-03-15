using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return await categories./*Include(d => d.Parent).*/ToListAsync();
        }
    }
    }

