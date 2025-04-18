using System.Linq.Expressions;
using DAL.Repositories.Base.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DAL.Repositories.Base.Implementation
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : BaseEntity
    {
        #region Contructor

        protected readonly IDbContextFactory<SecretContext> _contextFactory;
        public BaseRepository(IDbContextFactory<SecretContext> dbContextFactory)
        {
            _contextFactory = dbContextFactory;
        }

        #endregion

        #region GetAsync

        public virtual async Task<T> GetAsync(Guid uid)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            return await db.Set<T>().FindAsync(uid);
        }
        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            return await db.Set<T>().FirstOrDefaultAsync(predicate);
        }

        #endregion

        #region GetAllAsync

        public virtual async Task<List<T>> GetAllAsync()
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            return await db.Set<T>().ToListAsync();
        }
        public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            return await db.Set<T>().Where(predicate).ToListAsync();
        }

        #endregion

        #region Create
        public virtual async Task CreateAsync(T item)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            await db.Set<T>().AddAsync(item);
            await db.SaveChangesAsync();
        }
        #endregion

        #region Update
        public virtual async Task UpdateAsync(T item)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            db.Set<T>().Update(item);
            await db.SaveChangesAsync();
        }
        #endregion

        #region Delete
        public virtual async Task DeleteAsync(Guid uid)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            T item = await db.Set<T>().FindAsync(uid); 
            db.Set<T>().Remove(item);
            await db.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            List<T> items = await db.Set<T>().Where(predicate).ToListAsync();
            foreach (T item in items)
            {
                db.Set<T>().Remove(item);
            }
            await db.SaveChangesAsync();
        }
        #endregion
    }
}
