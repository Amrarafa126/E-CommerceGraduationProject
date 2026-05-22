using E_Commerce.Infrustructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_Commerce.Infrustructure.InfrustructureBases
{

    public class GenericRepositoryAsync<T>(AppDBContext context) : IGenericRepositoryAsync<T> where T : class
    {

        protected readonly AppDBContext Context = context;
        protected readonly DbSet<T> DbSet = context.Set<T>();

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await DbSet.FindAsync(new object[] { id }, ct);

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
            => await DbSet.AsNoTracking().ToListAsync(ct);

        public virtual async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await DbSet.AsNoTracking().Where(predicate).ToListAsync(ct);

        public virtual async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await DbSet.AsNoTracking().FirstOrDefaultAsync(predicate, ct);

        public virtual async Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await DbSet.AnyAsync(predicate, ct);

        public virtual async Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
            => predicate == null
                ? await DbSet.CountAsync(ct)
                : await DbSet.CountAsync(predicate, ct);

        public virtual async Task AddAsync(T entity, CancellationToken ct = default)
            => await DbSet.AddAsync(entity, ct);

        public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
            => await DbSet.AddRangeAsync(entities, ct);

        public virtual void Update(T entity)
        {
            var entry = Context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
                entry.State = EntityState.Modified;
            }
         
        }

        public virtual void Remove(T entity) => DbSet.Remove(entity);

        public virtual void RemoveRange(IEnumerable<T> entities) => DbSet.RemoveRange(entities);
    }
    }
