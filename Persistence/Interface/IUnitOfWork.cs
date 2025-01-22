using LibraryManagementDomain.Entities;
using Persistence.Interface;

namespace Persistence.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Book> Books { get; }      // Generic repository for Books
        IRepository<Author> Authors { get; } // Generic repository for Authors
        IRepository<Student> Students { get; } // Generic repository for Students
        IRepository<Course> Courses { get; }   // Generic repository for Courses
        Task<int> SaveChangesAsync();         // Save changes to the database
        Task BeginTransactionAsync();         // Begin a transaction
        Task CommitTransactionAsync();        // Commit a transaction
        void RollbackTransaction();           // Rollback a transaction
        Task SaveAndCommitAsync();            // Save and commit the transaction
    }
}
