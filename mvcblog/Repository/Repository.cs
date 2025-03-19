using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using mvcblog.Data;

namespace mvcblog.Repository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
        Task SaveChanges();
    }
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();

        }

        public async Task Add(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public IQueryable<T> GetAll()
        {
            IQueryable<T> all = dbSet;
            return all;
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }

        public async Task SaveChanges()
        {
            await _db.SaveChangesAsync();
        }
    }
}
