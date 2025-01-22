using Kernal;
using LibraryManagementApplication.Commands;
using LibraryManagementDomain.Models;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;
using System.Text.Json;
using Persistence.UnitOfWork;


public class AddAuthorRequest : IRequest<AuthorDTO>
{
    public AuthorModel Model { get; set; }

    public class AddAuthorRequestHandler : IRequestHandler<AddAuthorRequest, AuthorDTO>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRabbitMQSender<AuthorDTO> _rabbitMQSender;

        public AddAuthorRequestHandler(IMediator mediator, IUnitOfWork unitOfWork, IRabbitMQSender<AuthorDTO> rabbitMQSender)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _rabbitMQSender = rabbitMQSender;
        }

        public async Task<AuthorDTO> Handle(AddAuthorRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(); // Start transaction

            try
            {
                var dto = await _mediator.Send(new AddAuthorCommand { Model = request.Model }); // Send the command

                // Commit the transaction
                await _unitOfWork.SaveAndCommitAsync();
   
                // Send RabbitMQ message after adding the author
                _rabbitMQSender.SendMessage(
                    message: dto,
                    exchangeName: "amq.direct", // Default direct exchange
                    routingKey: "AuthorAdded",
                    queueName: "authorQueue"
                );

                Console.WriteLine("Message sent successfully!");

                return dto; // Return the DTO
            }
            catch (Exception ex)
            {
                // Rollback in case of error
                _unitOfWork.RollbackTransaction();
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
