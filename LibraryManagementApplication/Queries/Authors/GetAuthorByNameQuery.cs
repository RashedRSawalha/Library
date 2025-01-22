using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Redis;

namespace LibraryManagementApplication.Queries 
{ 

public class GetAuthorByNameQuery : IRequest<AuthorDTO>
{
    public string AuthorName { get; set; }

    public class Handler : IRequestHandler<GetAuthorByNameQuery, AuthorDTO>
    {
        private readonly IRepository<Author> _authorRepository;
        private readonly IRedisCache _redisCache;

        public Handler(IRepository<Author> authorRepository, IRedisCache redisCache)
        {
            _authorRepository = authorRepository;
            _redisCache = redisCache;
        }

        public async Task<AuthorDTO> Handle(GetAuthorByNameQuery request, CancellationToken cancellationToken)
        {
            
            var authorQuery = _authorRepository.GetAll()
                .Where(a => a.Name == request.AuthorName);
            var author = (await authorQuery.ToListAsync()).FirstOrDefault();

            if (author == null)
            {
                return null; // Author not found
            }

            var dto = author.ToDTO();

            // Store the fetched author in the Redis cache for future use
            //await _redisCache.SetAsync(request.AuthorName, TimeSpan.FromMinutes(100));

            return dto;

        }
    }
}
}