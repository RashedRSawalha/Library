using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.Models;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;

namespace LibraryManagementApplication.Queries { 

public class GetBooksQuery : IRequest<IEnumerable<BookDTO>>
{
    public class Handler : IRequestHandler<GetBooksQuery, IEnumerable<BookDTO>>
    {
        private readonly IRepository<Book> _bookRepository;

        public Handler(IRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<BookDTO>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
        {
            var books = await _bookRepository.GetAllAsync();


            var dto = books.Select(book => book.ToDTO());
            return dto;
        }
    }
}
}