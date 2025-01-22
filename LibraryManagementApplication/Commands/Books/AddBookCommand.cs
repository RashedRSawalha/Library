using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.Models;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;


namespace LibraryManagementApplication.Commands
{
    public class AddBookCommand : IRequest<BookDTO>
    {
        public BookModel Model { get; set; }

        public class AddBookCommandHandler : IRequestHandler<AddBookCommand, BookDTO>
        {
            private readonly IRepository<Book> _bookRepository;

            public AddBookCommandHandler(IRepository<Book> bookRepository)
            {
                _bookRepository = bookRepository;
            }

            public async Task<BookDTO> Handle(AddBookCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entity = request.Model.ToEntity(); // Convert model to entity
                    await _bookRepository.AddAsync(entity); // Add entity to the repository

                    var bookDTO = entity.ToDTO();

                    return bookDTO;


                    //return entity.ToDTO(); // Use ToDTO extension method to return DTO
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}