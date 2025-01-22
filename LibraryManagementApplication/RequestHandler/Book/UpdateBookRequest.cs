using Kernal;
using LibraryManagementDomain.Models;
using LibraryManagementApplication.Commands;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;
using System.Text.Json;
using Persistence.UnitOfWork;

public class UpdateBookRequest : IRequest<BookDTO>
{
    public int BookId { get; set; }
    public BookModel Model { get; set; }

    public class UpdateBookRequestHandler : IRequestHandler<UpdateBookRequest, BookDTO>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRabbitMQSender<BookDTO> _rabbitMQSender;

        public UpdateBookRequestHandler(IMediator mediator, IUnitOfWork unitOfWork, IRabbitMQSender<BookDTO> rabbitMQSender)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _rabbitMQSender = _rabbitMQSender;
        }

        public async Task<BookDTO> Handle(UpdateBookRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(); // Start transaction

            try
            {
                var dto = await _mediator.Send(new UpdateBookCommand
                {
                    BookId = request.BookId,
                    Model = request.Model
                }); // Send the command
                await _unitOfWork.SaveAndCommitAsync(); // Commit transaction

                _rabbitMQSender.SendMessage(
                    message: dto,
                    exchangeName: "amq.direct", // Default direct exchange
                    routingKey: "bookUpdated",
                    queueName: "bookQueue"
                );

                Console.WriteLine("Message sent successfully");

                return dto;
            }
            catch
            {
                _unitOfWork.RollbackTransaction(); // Rollback transaction on error
                throw;
            }
        }
    }
}
