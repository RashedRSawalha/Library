using Kernal;
using LibraryManagementDomain.DTO;
using System.Text.Json;

namespace WorkerService
{
    public class StudentWorker : BackgroundService
    {
        private readonly IRabbitMQReceiver<StudentDTO> _rabbitMQReceiver;
        private readonly ILogger<StudentWorker> _logger;

        public StudentWorker(IRabbitMQReceiver<StudentDTO> rabbitMQReceiver, ILogger<StudentWorker> logger)
        {
            _rabbitMQReceiver = rabbitMQReceiver;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StudentWorker is starting and listening for RabbitMQ messages...");

            // Start listening to the RabbitMQ queue
            _rabbitMQReceiver.Receive("studentQueue", async message =>
            {
                _logger.LogInformation($"Received Message: {message}");

                // Deserialize the message (if JSON payload is used)
                try
                {
                    // Log relevant fields from the deserialized StudentDTO
                    _logger.LogInformation($"Deserialized StudentDTO: Name: {message.StudentName}, StudentId: {message.StudentId}");
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Error deserializing message: {ex.Message}");
                }

                await Task.CompletedTask; // Add your processing logic here
            });

            return Task.CompletedTask;
        }
    }
}
