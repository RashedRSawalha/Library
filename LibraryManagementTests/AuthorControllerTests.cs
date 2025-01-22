using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagementAPI.Controllers;
using LibraryManagementDomain.DTO;
using LibraryManagementApplication.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using LibraryManagementInfrastructure;
using Moq;
using LibraryManagementApplication.Queries;
using LibraryManagementDomain.Models;

namespace LibraryManagementTests
{
    public class AuthorsControllerTests
    {
        private readonly LibraryDBContext _dbContext;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AuthorsController _controller;

        public AuthorsControllerTests()
        {
            var options = new DbContextOptionsBuilder<LibraryDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new LibraryDBContext(options);
            _mediatorMock = new Mock<IMediator>();
            _controller = new AuthorsController(
                unitOfWork: null,
                authorRepository: null,
                mediator: _mediatorMock.Object,
                redisCache: null,
                httpClient: null,
                tokenService: null,
                dBContext: _dbContext,
                logger:null
            );
        }

        // Test for GetAuthors
        [Fact]
        public async Task GetAuthors_ShouldReturnOkResult_WithAuthors()
        {
            // Arrange
            var authorsList = new List<AuthorDTO>
        {
            new AuthorDTO { AuthorId = 1, Name = "Author1" },
            new AuthorDTO { AuthorId = 2, Name = "Author2" }
        };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAuthorsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(authorsList);

            // Act
            var result = await _controller.GetAuthors();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAuthors = Assert.IsType<List<AuthorDTO>>(okResult.Value);
            Assert.Equal(2, returnedAuthors.Count);
        }

        // Test for AddAuthor
        [Fact]
        public async Task AddAuthor_ShouldAddAuthorAndReturnCreatedAtAction()
        {
            // Arrange
            var authorModel = new AuthorModel { Name = "New Author" };
            var createdAuthor = new AuthorDTO { AuthorId = 3, Name = "New Author" };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AddAuthorRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdAuthor);

            // Act
            var result = await _controller.AddAuthor(authorModel);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedAuthor = Assert.IsType<AuthorDTO>(createdResult.Value);
            Assert.Equal("New Author", returnedAuthor.Name);
        }

        // Test for UpdateAuthor
        [Fact]
        public async Task UpdateAuthor_ShouldUpdateAuthorAndReturnOkResult()
        {
            // Arrange
            var authorModel = new AuthorModel { Name = "Updated Author" };
            var updatedAuthor = new AuthorDTO { AuthorId = 1, Name = "Updated Author" };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateAuthorRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedAuthor);

            // Act
            var result = await _controller.UpdateAuthor(1, authorModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAuthor = Assert.IsType<AuthorDTO>(okResult.Value);
            Assert.Equal("Updated Author", returnedAuthor.Name);
        }

        // Test for DeleteAuthor
        [Fact]
        public async Task DeleteAuthor_ShouldRemoveAuthorAndReturnOk()
        {
            // Arrange
            const int authorId = 1;
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteAuthorRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(authorId);

            // Act
            var result = await _controller.DeleteAuthor(authorId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal($"Author with ID {authorId} deleted successfully.", okResult.Value);
        }
    }
}