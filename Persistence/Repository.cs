using LibraryManagementDomain.Entities;
using LibraryManagementInfrastructure;
using Microsoft.EntityFrameworkCore;
using Persistence.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly LibraryDBContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(LibraryDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet; // Return IQueryable for lazy loading
        }

        public async Task<List<T>> ToListAsync(IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            return await query.ToListAsync(cancellationToken); // Execute and convert to list
        }

        public async Task<T> FirstOrDefaultAsync(IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            return await query.FirstOrDefaultAsync(cancellationToken); // Fetch first or default
        }

        //public Task AddAsync(Book book)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
