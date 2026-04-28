using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Infrustructure.InfrustructureBases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface IUserRepos : IGenericRepositoryAsync<User>
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<User?> GetByRefreshTokenAsync(string token, CancellationToken ct = default);
        Task<User?> GetWithCompanyAsync(Guid userId, CancellationToken ct = default);
        Task<bool> ExistsAsync(Expression<Func<User, bool>> pred, CancellationToken ct = default);
        void Update(User user);
        void Remove(User user);
    }
}
