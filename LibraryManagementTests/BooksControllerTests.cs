using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementAPI.Controllers;
using LibraryManagementDomain.Models;
using LibraryManagementApplication.Queries;
using LibraryManagementApplication.Commands;
using MediatR;
using LibraryManagementDomain.DTO;

namespace LibraryManagementAPI.Tests
{
    public class BooksControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new BooksController(null, null, _mediatorMock.Object);
        }

        [Fact]
        public async Task GetBooks_ShouldReturnOkResult_WithBooks()
        {
            // Arrange
            var booksList = new List<BookDTO>
            {
                new BookDTO { BookId = 1, Title = "Book1" },
                new BookDTO { BookId = 2, Title = "Book2" }
            };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetBooksQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(booksList);

            // Act
            var result = await _controller.GetBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBooks = Assert.IsType<List<BookDTO>>(okResult.Value);
            Assert.Equal(2, returnedBooks.Count);
        }

        [Fact]
        public async Task AddBook_ShouldAddBookAndReturnCreatedAtAction()
        {
            // Arrange
            var bookModel = new BookModel { BookId = 1, Title = "New Book" };
            var addBookResponse = new BookDTO { BookId = 1 };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AddBookRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(addBookResponse);

            // Act
            var result = await _controller.AddBook(bookModel);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdBook = Assert.IsType<BookDTO>(createdAtResult.Value);
            Assert.Equal(1, createdBook.BookId);
        }

        [Fact]
        public async Task UpdateBook_ShouldUpdateBookAndReturnOk()
        {
            // Arrange
            var bookModel = new BookModel { BookId = 1, Title = "Updated Book" };
            var updatedBookResponse = new BookDTO { BookId = 1, Title = "Updated Book" };

            _mediatorMock
                .Setup(m => m.Send(It.Is<UpdateBookRequest>(r => r.BookId == 1 && r.Model.Title == "Updated Book"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedBookResponse);

            // Act
            var result = await _controller.UpdateBook(1, bookModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updatedBook = Assert.IsType<BookDTO>(okResult.Value);
            Assert.Equal("Updated Book", updatedBook.Title);
        }

        [Fact]
        public async Task DeleteBook_ShouldRemoveBookAndReturnOk()
        {
            // Arrange
            var deletedBookId = 1;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteBookRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(deletedBookId);

            // Act
            var result = await _controller.DeleteBook(deletedBookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains($"Book with ID {deletedBookId} deleted successfully.", okResult.Value.ToString());
        }
    }
}


