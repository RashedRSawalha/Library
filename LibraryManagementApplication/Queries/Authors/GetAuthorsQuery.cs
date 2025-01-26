using LibraryManagementDomain.DTO;
using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using Persistence.Interface;
using Shared.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryManagementApplication.Queries
{
    public class GetAuthorsQuery : IQuery<IEnumerable<AuthorDTO>>
    {
        public class Handler : IQueryHandler<GetAuthorsQuery, IEnumerable<AuthorDTO>>
        {
            private readonly IRepository<Author> _authorRepository;

            public Handler(IRepository<Author> authorRepository)
            {
                _authorRepository = authorRepository;
            }

            public async Task<IEnumerable<AuthorDTO>> Handle(GetAuthorsQuery query, CancellationToken cancellationToken)
            {
                // Fetch authors from repository
                var authors = await _authorRepository.GetAllAsync();

                // Map authors to DTOs
                return authors.Select(author => author.ToDTO());
            }
        }
    }
}
