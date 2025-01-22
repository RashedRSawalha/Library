using LibraryManagementDomain.Entities;
using Persistence.Interface;
using MediatR;


namespace LibraryManagementApplication.Commands
{
    public class DeleteAuthorCommand : IRequest<int>
    {
        public int AuthorId { get; set; }

        public DeleteAuthorCommand(int authorId)
        {
            AuthorId = authorId;
        }

        public class Handler : IRequestHandler<DeleteAuthorCommand, int>
        {
            private readonly IRepository<Author> _authorRepository;

            public Handler(IRepository<Author> authorRepository)
            {
                _authorRepository = authorRepository;
            }

            public async Task<int> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
            {
                // Fetch the author to delete
                var author = await _authorRepository.GetByIdAsync(request.AuthorId);
                if (author == null)
                    throw new KeyNotFoundException($"Author with ID {request.AuthorId} not found.");

                // Delete the author
                _authorRepository.Delete(author);
                return request.AuthorId; // Return the ID of the deleted author
            }
        }
    }
}