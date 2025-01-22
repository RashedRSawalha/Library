using Kernal;
using LibraryManagementApplication.Commands;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;
using System.Text.Json;
using Persistence.UnitOfWork;

public class DeleteBookRequest : IRequest<int>
{
    public int BookId { get; set; }

    public class DeleteBookRequestHandler : IRequestHandler<DeleteBookRequest, int>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRabbitMQSender<BookDTO> _rabbitMQSender;

        public DeleteBookRequestHandler(IMediator mediator, IUnitOfWork unitOfWork, IRabbitMQSender<BookDTO> rabbitMQSender)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _rabbitMQSender = rabbitMQSender;
        }

        public async Task<int> Handle(DeleteBookRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(); // Start transaction

            try
            {
                var deletedBookId = await _mediator.Send(new DeleteBookCommand(request.BookId)); // Call the command
                await _unitOfWork.SaveAndCommitAsync(); // Commit transaction

                _rabbitMQSender.SendMessage(
                    message: deletedBookId,
                    exchangeName: "amq.direct", // Default direct exchange
                    routingKey: "bookDeleted",
                    queueName: "bookQueue"
                    );

                return deletedBookId; // Return the deleted book ID
            }
            catch
            {
                _unitOfWork.RollbackTransaction(); // Rollback on error
                throw;
            }
        }
    }
}
