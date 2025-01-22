using Kernal;
using LibraryManagementApplication.Commands;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;
using System.Text.Json;
using Persistence.UnitOfWork;

public class DeleteAuthorRequest : IRequest<int>
{
    public int AuthorId { get; set; }

    public class DeleteAuthorRequestHandler : IRequestHandler<DeleteAuthorRequest, int>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRabbitMQSender<AuthorDTO> _rabbitMQSender;

        public DeleteAuthorRequestHandler(IMediator mediator, IUnitOfWork unitOfWork, IRabbitMQSender<AuthorDTO> rabbitMQSender)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _rabbitMQSender = rabbitMQSender;
        }

        public async Task<int> Handle(DeleteAuthorRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(); // Start transaction

            try
            {
                var deletedAuthorId = await _mediator.Send(new DeleteAuthorCommand(request.AuthorId)); // Call the command
                await _unitOfWork.SaveAndCommitAsync(); // Commit transaction

                _rabbitMQSender.SendMessage(
                    message: deletedAuthorId,
                    exchangeName: "amq.direct", // Default direct exchange
                    routingKey: "authorDeleted",
                    queueName: "authorQueue"
                    );

                return deletedAuthorId; // Return the deleted author ID
            }
            catch
            {
                _unitOfWork.RollbackTransaction(); // Rollback on error
                throw;
            }
        }
    }
}
