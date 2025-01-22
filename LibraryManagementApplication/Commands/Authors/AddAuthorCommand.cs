using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.Models;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;
using Shared.Redis;


namespace LibraryManagementApplication.Commands
{
    public class AddAuthorCommand : IRequest<AuthorDTO>
    {
        public AuthorModel Model { get; set; }

        public class AddAuthorCommandHandler : IRequestHandler<AddAuthorCommand, AuthorDTO>
        {
            private readonly IRepository<Author> _authorRepository;
            private readonly IRedisCache _redisCache;

            public AddAuthorCommandHandler(IRepository<Author> authorRepository, IRedisCache redisCache)
            {
                _authorRepository = authorRepository;
                _redisCache = redisCache;
            }

            public async Task<AuthorDTO> Handle(AddAuthorCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    // Convert the model to an entity
                    var entity = request.Model.ToEntity();

                    // Add the entity to the database
                    await _authorRepository.AddAsync(entity);


                    // Map the saved entity to a DTO
                    var authorDTO = entity.ToDTO();

                    // Cache the author using their name as the key
                    string cacheKey = $"Author:{authorDTO.Name}";
                    await _redisCache.SetAsync(cacheKey, authorDTO, TimeSpan.FromMinutes(100));

                    // Return the DTO
                    return authorDTO;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"Error adding author: {ex.Message}");
                }
            }
        }
    }
}