using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.Models;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;

namespace LibraryManagementApplication.Commands
{

    public class UpdateBookCommand : IRequest<BookDTO>
    {
        public int BookId { get; set; }
        public BookModel Model { get; set; }

        public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDTO>
        {
            private readonly IRepository<Book> _bookRepository;

            public UpdateBookCommandHandler(IRepository<Book> bookRepository)
            {
                _bookRepository = bookRepository;
            }

            public async Task<BookDTO> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
            {
                var existingBook = await _bookRepository.GetByIdAsync(request.BookId);
                if (existingBook == null)
                    throw new KeyNotFoundException($"Book with ID {request.BookId} not found.");

                existingBook.Title = request.Model.Title;
                existingBook.AuthorId = request.Model.AuthorId;

                _bookRepository.Update(existingBook);

                return existingBook.ToDTO(); // Use ToDTO extension method
            }
        }
    }
}