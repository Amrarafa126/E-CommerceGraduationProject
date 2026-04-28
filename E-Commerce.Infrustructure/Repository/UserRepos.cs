using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Repository
{
    public class UserRepos(AppDBContext db) : GenericRepositoryAsync<User>(db), IUserRepos
    {
        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
         => db.Users.FindAsync([id], ct).AsTask();

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
            => db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower(), ct);

        public Task<User?> GetByRefreshTokenAsync(string token, CancellationToken ct = default)
            => db.Users.FirstOrDefaultAsync(u => u.RefreshToken == token, ct);

        public Task<User?> GetWithCompanyAsync(Guid userId, CancellationToken ct = default)
            => db.Users
                .Include(u => u.OwnedCompany).ThenInclude(c => c!.Wallet)
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

        public Task<bool> ExistsAsync(
            Expression<Func<User, bool>> pred, CancellationToken ct = default)
            => db.Users.AnyAsync(pred, ct);

        public void Update(User user)
        {
            db.Users.Attach(user);
            db.Entry(user).State = EntityState.Modified;
        }

    }
}
