using Kernal;
using LibraryManagementApplication.Commands;
using LibraryManagementDomain.Models;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;
using System.Text.Json;
using Persistence.UnitOfWork;


public class AddCourseRequest : IRequest<CourseDTO>
{
    public CourseModel Model { get; set; }

    public class AddCourseRequestHandler : IRequestHandler<AddCourseRequest, CourseDTO>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRabbitMQSender<CourseDTO> _rabbitMQSender;

        public AddCourseRequestHandler(IMediator mediator, IUnitOfWork unitOfWork, IRabbitMQSender<CourseDTO> rabbitMQSender)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _rabbitMQSender = rabbitMQSender;
        }

        public async Task<CourseDTO> Handle(AddCourseRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(); // Start transaction

            try
            {
                var dto = await _mediator.Send(new AddCourseCommand { Model = request.Model }); // Send the command

                // Commit the transaction
                await _unitOfWork.SaveAndCommitAsync();

                // Send RabbitMQ message after adding the course
                _rabbitMQSender.SendMessage(
                    message: dto,
                    exchangeName: "amq.direct", // Default direct exchange
                    routingKey: "CourseAdded",
                    queueName: "courseQueue"
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
