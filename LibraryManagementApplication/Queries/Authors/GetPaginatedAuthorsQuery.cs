using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace LibraryManagementApplication.Queries
{

    public class GetPaginatedAuthorsQuery : IRequest<PaginatedAuthorDTO>
    {
        public int PageIndex { get; set; } // Current page index (1-based)
        public int PageSize { get; set; } // Number of items per page

        public string? Search { get; set; } // Optional search term

        public class Handler : IRequestHandler<GetPaginatedAuthorsQuery, PaginatedAuthorDTO>
        {
            private readonly IRepository<Author> _authorRepository;

            public Handler(IRepository<Author> authorRepository)
            {
                _authorRepository = authorRepository;
            }

            public async Task<PaginatedAuthorDTO> Handle(GetPaginatedAuthorsQuery request, CancellationToken cancellationToken)
            {
                var query = _authorRepository.GetAll();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    query = query.Where(a => a.Name.Contains(request.Search));
                }

                // Total number of records
                var totalRecords = await query.CountAsync(cancellationToken);

                // Paginated data
                var authors = await query
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                // Map to DTO
                var authorDTOs = authors.Select(author => author.ToDTO()).ToList();

                // Return paginated result
                return new PaginatedAuthorDTO
                {
                    TotalRecords = totalRecords,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Authors = authorDTOs.ToList()
                };
            }
        }
    }
}