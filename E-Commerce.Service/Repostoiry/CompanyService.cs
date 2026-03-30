
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Interfase;
using E_Commerce.Infrustructure.Repository;
using E_Commerce.Service.Interfase;

namespace E_Commerce.Service.Repostoiry
{
    public class CompanyService : ICompanyService
    {
        ICompanyRepos CompanyRepos;
        public CompanyService(ICompanyRepos companyRepos)
        {
           CompanyRepos = companyRepos;
        }
        public async Task<string> AddCategoryAsync(Company company)
        {

            var CompanyCheak = CompanyRepos.GetTableNoTracking().Where(c => c.CompanyName == company.CompanyName).FirstOrDefault();
            if (CompanyCheak != null) return "Exist";

            var AddCompany = await CompanyRepos.AddAsync(company);
            if (AddCompany == null) return "Failed Add";
            return "Success";

        }

        public Task<List<Company>> GetCompanyListAsync()
        {
            throw new NotImplementedException();
        }
    }
}
