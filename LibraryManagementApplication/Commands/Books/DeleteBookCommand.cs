using LibraryManagementDomain.Entities;
using Persistence.Interface;
using MediatR;

namespace LibraryManagementApplication.Commands
{

    public class DeleteBookCommand : IRequest<int>
    {
        public int BookId { get; set; }

        public DeleteBookCommand(int bookId)
        {
            BookId = bookId;
        }

        public class Handler : IRequestHandler<DeleteBookCommand, int>
        {
            private readonly IRepository<Book> _bookRepository;

            public Handler(IRepository<Book> bookRepository)
            {
                _bookRepository = bookRepository;
            }

            public async Task<int> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
            {
                // Fetch the book to delete
                var book = await _bookRepository.GetByIdAsync(request.BookId);
                if (book == null)
                    throw new KeyNotFoundException($"Book with ID {request.BookId} not found.");

                // Delete the book
                _bookRepository.Delete(book);
                return request.BookId; // Return the ID of the deleted book
            }
        }
    }
}