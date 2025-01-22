using Kernal;
using LibraryManagementDomain.Models;
using LibraryManagementDomain.DTO;
using LibraryManagementApplication.Commands;
using Persistence.Interface;
using MediatR;
using System.Text.Json;
using Persistence.UnitOfWork;

public class AddBookRequest : IRequest<BookDTO>
{
    public BookModel Model { get; set; }

    public class AddBookRequestHandler : IRequestHandler<AddBookRequest, BookDTO>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRabbitMQSender<BookDTO> _rabbitMQSender;

        public AddBookRequestHandler(IMediator mediator, IUnitOfWork unitOfWork, IRabbitMQSender<BookDTO> rabbitMQSender)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _rabbitMQSender = rabbitMQSender;
        }

        public async Task<BookDTO> Handle(AddBookRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(); // Start transaction

            try
            {
                // Send the command via MediatR
                var dto = await _mediator.Send(new AddBookCommand { Model = request.Model });
                await _unitOfWork.SaveAndCommitAsync(); // Commit transaction
           
                _rabbitMQSender.SendMessage(
                    message: dto,
                    exchangeName: "amq.direct", // Default direct exchange
                    routingKey: "bookAdded",
                    queueName: "bookQueue"
                    );

                Console.WriteLine("Message sent successfully");

                return dto;
            }
            catch
            {
                _unitOfWork.RollbackTransaction(); // Rollback transaction in case of error
                throw;
            }
        }
    }
}
