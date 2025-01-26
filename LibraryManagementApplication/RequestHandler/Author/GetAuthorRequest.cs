using LibraryManagementApplication.Queries;
using LibraryManagementDomain.DTO;
using MediatR;
using Shared.Helpers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryManagementApplication.Requests
{
    public class GetAuthorsRequest : IRequest<IEnumerable<AuthorDTO>>
    {
        public class Handler : IRequestHandler<GetAuthorsRequest, IEnumerable<AuthorDTO>>
        {
            private readonly Dispatcher _dispatcher;

            public Handler(Dispatcher dispatcher)
            {
                _dispatcher = dispatcher;
            }

            public async Task<IEnumerable<AuthorDTO>> Handle(GetAuthorsRequest request, CancellationToken cancellationToken)
            {
                // Dispatch the GetAuthorsQuery via the Dispatcher
                return await _dispatcher.DispatchAsync(new GetAuthorsQuery(), cancellationToken);
            }
        }
    }
}
