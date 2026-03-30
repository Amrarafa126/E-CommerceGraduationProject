
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InfrustructureBases;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface ICompanyRepos : IGenericRepositoryAsync<Company>
    {
        public Task<List<Company>> GetCompanyListAsync();
    }
}
