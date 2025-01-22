using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.Models;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;


namespace LibraryManagementApplication.Commands
{
    public class UpdateAuthorCommand : IRequest<AuthorDTO>
    {
        public int AuthorId { get; set; }
        public AuthorModel Model { get; set; }

        public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, AuthorDTO>
        {
            private readonly IRepository<Author> _authorRepository;

            public UpdateAuthorCommandHandler(IRepository<Author> authorRepository)
            {
                _authorRepository = authorRepository;
            }

            public async Task<AuthorDTO> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
            {
                var existingAuthor = await _authorRepository.GetByIdAsync(request.AuthorId);
                if (existingAuthor == null)
                    return null;
                //throw new KeyNotFoundException($"Author with ID {request.AuthorId} not found.");

                existingAuthor.Name = request.Model.Name;
                existingAuthor.AuthorAge = request.Model.AuthorAge;
                existingAuthor.AuthorType = request.Model.AuthorType;
                _authorRepository.Update(existingAuthor);



                return existingAuthor.ToDTO(); // Use ToDTO extension method
            }
        }
    }
}