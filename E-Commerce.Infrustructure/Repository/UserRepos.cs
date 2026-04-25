using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Repository
{
    public class UserRepos : GenericRepositoryAsync<User>, IUserRepos
    {
        DbSet<User> users;
        public UserRepos(AppDBContext dbContext) : base(dbContext)
        {
            users = dbContext.Set<User>();
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetWithCompanyAsync(Guid userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
