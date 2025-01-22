using Kernal;
using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Models;
using LibraryManagementApplication.Authentication;
using LibraryManagementApplication.Commands;
using LibraryManagementDomain.DTO;
using LibraryManagementApplication.Queries;
using Persistence.Interface;
using Persistence.UnitOfWork;
using LibraryManagementApplication.Authorization;
using LibraryManagementApplication.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Redis;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using LibraryManagementInfrastructure;
using System.Web.Helpers;
using BCrypt.Net;
//using LibraryManagementAPI.Data;
using Shared.AuthorizationMiddleware;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Book> _bookRepository;
        private readonly IMediator _mediator;

        public BooksController(IUnitOfWork unitOfWork, IRepository<Book> bookRepository,IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _bookRepository = bookRepository;
            _mediator = mediator;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookModel>>> GetBooks()
        {
            var books = await _mediator.Send(new GetBooksQuery());
            return Ok(books);
        }

        // GET: api/books/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BookModel>> GetBookById(int id)
        {
            var book = await _mediator.Send(new GetBookByIdQuery(id));
            if (book == null)return NotFound();
            return Ok(book);
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult> AddBook([FromBody] BookModel model)
        {
            var createdBook = await _mediator.Send(new AddBookRequest { Model = model });
            return CreatedAtAction(nameof(AddBook), new { id = createdBook.BookId }, createdBook);
        }


        // PUT: api/books/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookModel model)
        {
            var updatedBook = await _mediator.Send(new UpdateBookRequest{BookId = id,Model = model});
            return Ok(updatedBook);
        }


        // DELETE: api/books/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var deletedBookId = await _mediator.Send(new DeleteBookRequest { BookId = id });
            return Ok($"Book with ID {deletedBookId} deleted successfully.");
        }

    }
}

