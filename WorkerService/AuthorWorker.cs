using Kernal;
using LibraryManagementDomain.DTO;
using System.Text.Json;

namespace WorkerService
{
    public class AuthorWorker : BackgroundService
    {
        private readonly IRabbitMQReceiver<AuthorDTO> _rabbitMQReceiver;
        private readonly ILogger<AuthorWorker> _logger;

        public AuthorWorker(IRabbitMQReceiver<AuthorDTO> rabbitMQReceiver, ILogger<AuthorWorker> logger)
        {
            _rabbitMQReceiver = rabbitMQReceiver;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker is starting and listening for RabbitMQ messages...");

            // Start listening to the RabbitMQ queue
            _rabbitMQReceiver.Receive("authorQueue", async message =>
            {
                _logger.LogInformation($"Received Message: {message}");

                // Deserialize the message (if JSON payload is used)
                try
                {
                    //var checkedmessage = JsonSerializer.Deserialize<StudentDTO>(message);
                    _logger.LogInformation($"Deserialized Message: {message.AuthorId}");
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
