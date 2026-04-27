
//using E_Commerce.Data.Entity;
//using E_Commerce.Infrustructure.Interfase;
//using E_Commerce.Service.Interfase;


//namespace E_Commerce.Service.Repostoiry
//{
//    public class CategoryService :ICategoryService
//    {
//        ICategoryRepos CategoryRepos;
//        public CategoryService(ICategoryRepos CategoryRepos) 
//        {
//            this.CategoryRepos = CategoryRepos;
        
//        }

//        public async Task<string> AddCategoryAsync(Category category)
//        {
          
//            var CategoryCheak = CategoryRepos.GetTableNoTracking().Where(c => c.NameCategory == category.NameCategory).FirstOrDefault();
//            if (CategoryCheak != null) return "Exist";

//             var addCategory =   await CategoryRepos.AddAsync(category);
//                if (addCategory == null) return "Failed Add";
//            return "Success";

//        }

//        public async Task<string> DeleteCategoryAsync(Category category)
//        {
//            var trun = CategoryRepos.BeginTransaction();
//            try
//            {
//                await CategoryRepos.DeleteAsync(category);
//                await trun.CommitAsync();
//                return "Success";
//            }
//            catch
//            {
//                await trun.RollbackAsync();
//                return "failed";
//            }
//        }

//        public Task<string> EditCategoryAsync(Category category)
//        {
//            var EditCategory = CategoryRepos.UpdateAsync(category);
//            if (EditCategory == null) return Task.FromResult("Failed Edit");
//            return Task.FromResult("Success");

//        }

//        public async Task<Category> GetCategoryByIdAsync(int id)
//        {
//             var GetCategory = await CategoryRepos.GetByIdAsync(id);
//            return GetCategory;
//        }

//        public async Task<List<Category>> GetCategoryListAsync()
//        {
//            return await CategoryRepos.GetCategoryListAsync();
//        }

//    }
//}
