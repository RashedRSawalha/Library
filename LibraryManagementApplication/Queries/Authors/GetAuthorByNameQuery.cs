using LibraryManagementDomain.Entities;
using LibraryManagementDomain.DTO;
using LibraryManagementDomain.Mapping;
using Persistence.Interface;
using Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementApplication.Queries
{
    public class GetAuthorByNameQuery : IQuery<AuthorDTO>
    {
        public string AuthorName { get; set; }

        public class Handler : IQueryHandler<GetAuthorByNameQuery, AuthorDTO>
        {
            private readonly IRepository<Author> _authorRepository;

            public Handler(IRepository<Author> authorRepository)
            {
                _authorRepository = authorRepository;
            }

            public async Task<AuthorDTO> Handle(GetAuthorByNameQuery query, CancellationToken cancellationToken)
            {
                var author = await _authorRepository.GetAll()
                    .Where(a => a.Name == query.AuthorName)
                    .FirstOrDefaultAsync(cancellationToken);

                if (author == null)
                    return null;

                return author.ToDTO();
            }
        }
    }
}
