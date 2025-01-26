using LibraryManagementDomain.Entities;
using LibraryManagementDomain.DTO;
using LibraryManagementInfrastructure;
using Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Persistence.Interface;

namespace LibraryManagementTests
{
    public class AuthorRepositoryTests
    {
        private readonly LibraryDBContext _dbContext;
        private readonly Repository<Author> _authorRepository;

        public AuthorRepositoryTests()
        {
            // Use In-Memory Database for Testing
            var options = new DbContextOptionsBuilder<LibraryDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test run
                .Options;

            _dbContext = new LibraryDBContext(options);

            // Create a new instance of the repository
            _authorRepository = new Repository<Author>(_dbContext);
        }

        [Fact]
        public async Task AddAsync_ShouldAddAuthorToDatabase()
        {
            // Arrange
            var author = new Author
            {
                Name = "Test Author",
                AuthorAge = 35,
                AuthorType = 1
            };

            // Act
            await _authorRepository.AddAsync(author);
            await _dbContext.SaveChangesAsync();

            // Assert
            var dbAuthor = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Name == "Test Author");
            Assert.NotNull(dbAuthor);
            Assert.Equal("Test Author", dbAuthor.Name);
        }

        [Fact]
        public async Task GetAllAuthors_ShouldReturnCorrectCount()
        {
            // Arrange
            var authorsList = new List<Author>
    {
        new Author { Name = "Author 1" },
        new Author { Name = "Author 2" }
    };

            var mockRepository = new Mock<IRepository<Author>>();
            mockRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(authorsList);

            var repository = mockRepository.Object;

            // Act
            var allAuthors = await repository.GetAllAsync();

            // Assert
            Assert.Equal(2, allAuthors.Count()); // Use Count() for IEnumerable
        }


        [Fact]
        public async Task GetByIdAsync_ShouldReturnAuthorById()
        {
            // Arrange
            var author = new Author
            {
                Name = "Author1",
                AuthorAge = 40,
                AuthorType = 1
            };

            await _dbContext.Authors.AddAsync(author);
            await _dbContext.SaveChangesAsync();

            // Act
            var dbAuthor = await _authorRepository.GetByIdAsync(author.AuthorId);

            // Assert
            Assert.NotNull(dbAuthor);
            Assert.Equal(author.Name, dbAuthor.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAuthorInDatabase()
        {
            // Arrange
            var author = new Author
            {
                Name = "Author1",
                AuthorAge = 40,
                AuthorType = 1
            };

            await _dbContext.Authors.AddAsync(author);
            await _dbContext.SaveChangesAsync();

            // Act
            author.Name = "Updated Author";
            _authorRepository.Update(author);
            await _dbContext.SaveChangesAsync();

            // Assert
            var updatedAuthor = await _dbContext.Authors.FirstOrDefaultAsync(a => a.AuthorId == author.AuthorId);
            Assert.NotNull(updatedAuthor);
            Assert.Equal("Updated Author", updatedAuthor.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveAuthorFromDatabase()
        {
            // Arrange
            var author = new Author
            {
                Name = "Author1",
                AuthorAge = 40,
                AuthorType = 1
            };

            await _dbContext.Authors.AddAsync(author);
            await _dbContext.SaveChangesAsync();

            // Act
            _authorRepository.Delete(author);
            await _dbContext.SaveChangesAsync();

            // Assert
            var deletedAuthor = await _dbContext.Authors.FirstOrDefaultAsync(a => a.AuthorId == author.AuthorId);
            Assert.Null(deletedAuthor);
        }

        [Fact]
        public async Task AnyAsync_ShouldReturnTrueIfAuthorExists()
        {
            // Arrange
            var author = new Author
            {
                Name = "Author1",
                AuthorAge = 40,
                AuthorType = 1
            };

            await _dbContext.Authors.AddAsync(author);
            await _dbContext.SaveChangesAsync();

            // Act
            var exists = await _authorRepository.AnyAsync(a => a.Name == "Author1");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task AnyAsync_ShouldReturnFalseIfAuthorDoesNotExist()
        {
            // Act
            var exists = await _authorRepository.AnyAsync(a => a.Name == "NonExistentAuthor");

            // Assert
            Assert.False(exists);
        }
    }
}
