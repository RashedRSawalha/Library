using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.Models;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;

namespace LibraryManagementApplication.Queries { 

public class GetBookByIdQuery : IRequest<BookDTO>
{
    public int BookId { get; set; }

    public GetBookByIdQuery(int bookId)
    {
        BookId = bookId;
    }

    public class Handler : IRequestHandler<GetBookByIdQuery, BookDTO>
    {
        private readonly IRepository<Book> _bookRepository;

        public Handler(IRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookDTO> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.BookId);

            if (book == null)
                return null;

            var dto = book.ToDTO();
            return dto;
        }
    }
}
    }