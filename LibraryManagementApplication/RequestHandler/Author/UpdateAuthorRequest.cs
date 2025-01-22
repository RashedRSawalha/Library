using Kernal;
using LibraryManagementDomain.Models;
using LibraryManagementApplication.Commands;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;
using Persistence.UnitOfWork;

public class UpdateAuthorRequest : IRequest<AuthorDTO>
{
    public int AuthorId { get; set; }
    public AuthorModel Model { get; set; }

    public class UpdateAuthorRequestHandler : IRequestHandler<UpdateAuthorRequest, AuthorDTO>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRabbitMQSender<AuthorDTO> _rabbitMQSender;
        

        public UpdateAuthorRequestHandler(IMediator mediator, IUnitOfWork unitOfWork, IRabbitMQSender<AuthorDTO> rabbitMQSender)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _rabbitMQSender = rabbitMQSender;
        }

        public async Task<AuthorDTO> Handle(UpdateAuthorRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(); // Start transaction

            try
            {
                var dto = await _mediator.Send(new UpdateAuthorCommand
                {
                    AuthorId = request.AuthorId,
                    Model = request.Model
                }); // Send the command
                await _unitOfWork.SaveAndCommitAsync(); // Commit the transaction

                _rabbitMQSender.SendMessage(
                    message: dto,
                    exchangeName: "amq.direct", // Default direct exchange
                    routingKey: "authorUpdated",
                    queueName: "authorQueue"
                );

                Console.WriteLine("Message sent successfully");

                return dto;
            }
            catch 
            {
                _unitOfWork.RollbackTransaction(); // Rollback in case of error
                throw;
            }
        }
    }
}
