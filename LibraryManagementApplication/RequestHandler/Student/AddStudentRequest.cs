using Kernal;
using LibraryManagementDomain.Models;
using LibraryManagementDomain.DTO;
using LibraryManagementApplication.Commands;
using Persistence.Interface;
using MediatR;
using System.Text.Json;
using Persistence.UnitOfWork;

public class AddStudentRequest : IRequest<StudentDTO>
{
    public StudentModel Model { get; set; }

    public class AddStudentRequestHandler : IRequestHandler<AddStudentRequest, StudentDTO>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRabbitMQSender<StudentDTO> _rabbitMQSender;

        public AddStudentRequestHandler(IMediator mediator, IUnitOfWork unitOfWork, IRabbitMQSender<StudentDTO> rabbitMQSender)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _rabbitMQSender = rabbitMQSender;
        }

        public async Task<StudentDTO> Handle(AddStudentRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(); // Start transaction

            try
            {
                var dto = await _mediator.Send(new AddStudentCommand { Model = request.Model });
                await _unitOfWork.SaveAndCommitAsync();

                _rabbitMQSender.SendMessage(
                    message: dto,
                    exchangeName: "amq.direct", // Default direct exchange
                    routingKey: "StudentAdded",
                    queueName: "studentQueue"
                    );

                Console.WriteLine("Message sent successfully");

                return dto;
            }
            catch
            {
                _unitOfWork.RollbackTransaction(); // Rollback in case of an error
                throw;
            }
        }
    }
}
