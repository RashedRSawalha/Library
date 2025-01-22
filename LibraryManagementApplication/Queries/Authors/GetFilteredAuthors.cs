using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;

namespace LibraryManagementApplication.Queries

{
    public class GetFilteredAuthorsQuery : IRequest<IEnumerable<AuthorDTO>>
    {
        public string Search { get; set; } = string.Empty; // Required search term
    }

    public class GetFilteredAuthorsHandler : IRequestHandler<GetFilteredAuthorsQuery, IEnumerable<AuthorDTO>>
    {
        private readonly IRepository<Author> _authorRepository;

        public GetFilteredAuthorsHandler(IRepository<Author> authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<IEnumerable<AuthorDTO>> Handle(GetFilteredAuthorsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Search))
            {
                return Enumerable.Empty<AuthorDTO>(); // Return an empty list if search is invalid
            }

            // Use IQueryable to filter authors
            var query = _authorRepository.GetAll()
                .Where(a => a.Name.ToLower().Contains(request.Search.ToLower()));


            // Execute the query and convert to a list
            var authors = await _authorRepository.ToListAsync(query, cancellationToken);

            // Convert to DTOs
            return authors.Select(a => a.ToDTO()).ToList();
        }
    }
}
