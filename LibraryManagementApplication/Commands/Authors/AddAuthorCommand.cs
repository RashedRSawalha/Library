using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.Models;
using LibraryManagementDomain.DTO;
using LibraryManagementApplication.RequestHandler;
using Persistence.Interface;
using Shared.Redis;
using Shared.Contracts;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementApplication.Commands
{
    public class AddAuthorCommand : ICommand
    {
        public AuthorModel Model { get; set; }

        public class AddAuthorCommandHandler : ICommandHandler<AddAuthorCommand>
        {
            private readonly IRepository<Author> _authorRepository;
            private readonly IRedisCache _redisCache;

            public AddAuthorCommandHandler(
                IRepository<Author> authorRepository,
                IRedisCache redisCache)
            {
                _authorRepository = authorRepository;
                _redisCache = redisCache;
            }

            public async Task Handle(AddAuthorCommand command, CancellationToken cancellationToken)
            {
                if (await _authorRepository.AnyAsync(a => a.Name == command.Model.Name))
                {
                    throw new ValidationException($"Author with the name '{command.Model.Name}' already exists.");
                }


                // Convert model to entity and save
                var entity = command.Model.ToEntity();
                await _authorRepository.AddAsync(entity);

                // Cache the new author
                string cacheKey = $"Author:{entity.Name}";
                await _redisCache.SetAsync(cacheKey, entity.ToDTO(), TimeSpan.FromMinutes(100));
            }
        }
    }

}