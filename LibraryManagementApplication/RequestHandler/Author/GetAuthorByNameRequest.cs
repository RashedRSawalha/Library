using LibraryManagementDomain.DTO;
using Shared.Helpers;
using MediatR;
using LibraryManagementApplication.Queries;

namespace LibraryManagementApplication.Requests
{
    public class GetAuthorByNameRequest : IRequest<AuthorDTO>
    {
        public string AuthorName { get; set; }

        public class Handler : IRequestHandler<GetAuthorByNameRequest, AuthorDTO>
        {
            private readonly Dispatcher _dispatcher;

            public Handler(Dispatcher dispatcher)
            {
                _dispatcher = dispatcher;
            }

            public async Task<AuthorDTO> Handle(GetAuthorByNameRequest request, CancellationToken cancellationToken)
            {
                var query = new GetAuthorByNameQuery { AuthorName = request.AuthorName };
                return await _dispatcher.DispatchAsync(query, cancellationToken);
            }
        }
    }
}
