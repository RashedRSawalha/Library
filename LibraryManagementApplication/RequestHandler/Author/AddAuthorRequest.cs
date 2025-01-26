using Kernal;
using LibraryManagementDomain.Mapping;
using Shared.Helpers;
using LibraryManagementDomain.DTO;
using LibraryManagementDomain.Models;
using LibraryManagementApplication.Commands;
using MediatR;
using Persistence.UnitOfWork;

namespace LibraryManagementApplication.RequestHandler
{

    public class AddAuthorRequest : IRequest<AuthorDTO>
    {
        public AuthorModel Model { get; set; }

        public class AddAuthorRequestHandler : IRequestHandler<AddAuthorRequest, AuthorDTO>
        {
            private readonly Dispatcher _dispatcher;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IRabbitMQSender<AuthorDTO> _rabbitMQSender;

            public AddAuthorRequestHandler(
                Dispatcher dispatcher,
                IUnitOfWork unitOfWork,
                IRabbitMQSender<AuthorDTO> rabbitMQSender)
            {
                _dispatcher = dispatcher;
                _unitOfWork = unitOfWork;
                _rabbitMQSender = rabbitMQSender;
            }

            public async Task<AuthorDTO> Handle(AddAuthorRequest request, CancellationToken cancellationToken)
            {
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Dispatch AddAuthorCommand
                    var addAuthorCommand = new AddAuthorCommand { Model = request.Model };
                    await _dispatcher.DispatchAsync(addAuthorCommand, cancellationToken);

                    // Commit transaction
                    await _unitOfWork.SaveAndCommitAsync();

                    // Map to DTO
                    var authorEntity = request.Model.ToEntity();
                    var dto = authorEntity.ToDTO();

                    // Send RabbitMQ message
                    _rabbitMQSender.SendMessage(
                        message: dto,
                        exchangeName: "amq.direct",
                        routingKey: "AuthorAdded",
                        queueName: "authorQueue"
                    );

                    return dto;
                }
                catch (Exception ex)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new ApplicationException($"Error adding author: {ex.Message}", ex);
                }
            }
        }
    }
}