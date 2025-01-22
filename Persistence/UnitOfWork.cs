using LibraryManagementDomain.Entities;
//using LibraryManagementAPI.Data;
using LibraryManagementInfrastructure;
using Persistence.Interface;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Repository;

namespace Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly LibraryDBContext _context;
        private IDbContextTransaction _transaction;

        public IRepository<Book> Books { get; private set; }       // Generic repository for Books
        public IRepository<Author> Authors { get; private set; }  // Generic repository for Authors
        public IRepository<Student> Students { get; private set; } // Repository for Students
        public IRepository<Course> Courses { get; private set; }   // Repository for Courses



        //IRepository<BookModel> IUnitOfWork.Books => throw new NotImplementedException();

        //IRepository<AuthorModel> IUnitOfWork.Authors => throw new NotImplementedException();

        public UnitOfWork(LibraryDBContext context)
        {
            _context = context;
            //Books = new Repository<Book>(context);     // Initialize generic repository for Books
            //Authors = new Repository<Author>(context);   // Initialize generic repository for Authors
            //Students = new Repository<Student>(context);
            //Courses = new Repository<Course>(context);

        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync(); // Save changes to the database
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync(); // Start a transaction
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(); // Commit the transaction
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback(); // Rollback the transaction
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _context.Dispose(); // Dispose of the DbContext
        }

        public async Task SaveAndCommitAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction not started");

            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
            //await _transaction.DisposeAsync();
            _transaction = null;
        }

    }
}
