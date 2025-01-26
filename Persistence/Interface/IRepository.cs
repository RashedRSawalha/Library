using System.Linq.Expressions;

namespace Persistence.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        //Task AddAsync(Book book); 

        IQueryable<T> GetAll(); // For lazy-loading
        Task<List<T>> ToListAsync(IQueryable<T> query, CancellationToken cancellationToken = default); // Convert IQueryable to List
        Task<T> FirstOrDefaultAsync(IQueryable<T> query, CancellationToken cancellationToken = default); // Fetch first or default
    }
}
