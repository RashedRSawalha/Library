using Kernal;
using LibraryManagementDomain.DTO;
using System.Text.Json;

namespace WorkerService
{
    public class CourseWorker : BackgroundService
    {
        private readonly IRabbitMQReceiver<CourseDTO> _rabbitMQReceiver;
        private readonly ILogger<CourseWorker> _logger;

        public CourseWorker(IRabbitMQReceiver<CourseDTO> rabbitMQReceiver, ILogger<CourseWorker> logger)
        {
            _rabbitMQReceiver = rabbitMQReceiver;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CourseWorker is starting and listening for RabbitMQ messages...");

            // Start listening to the RabbitMQ queue
            _rabbitMQReceiver.Receive("courseQueue", async message =>
            {
                _logger.LogInformation($"Received Message: {message}");

                // Deserialize the message (if JSON payload is used)
                try
                {
                    // Log relevant fields from the deserialized CourseDTO
                    _logger.LogInformation($"Deserialized CourseDTO: Title: {message.Title}, CourseId: {message.CourseId}");
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
