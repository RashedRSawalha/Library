using LibraryManagementDomain.Entities;
using LibraryManagementDomain.DTO;
using LibraryManagementDomain.Mapping;
using Persistence.Interface;
using MediatR;

namespace LibraryManagementApplication.Queries
{
    public class GetAuthorsQuery : IRequest<IEnumerable<AuthorDTO>>
    {
        public class Handler : IRequestHandler<GetAuthorsQuery, IEnumerable<AuthorDTO>>
        {
            private readonly IRepository<Author> _authorRepository;

            public Handler(IRepository<Author> authorRepository)
            {
                _authorRepository = authorRepository;
            }

            public async Task<IEnumerable<AuthorDTO>> Handle(GetAuthorsQuery request, CancellationToken cancellationToken)
            {
                var authors = await _authorRepository.GetAllAsync();

                // Map authors to DTOs
                var dto = authors.Select(author => author.ToDTO());
                return dto;
            }
        }
    }
}
