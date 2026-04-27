using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Infrustructure.InfrustructureBases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface IUserRepos : IGenericRepositoryAsync<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
        Task<User?> GetWithCompanyAsync(Guid userId, CancellationToken ct = default);
    }
}
