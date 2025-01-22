using Kernal;
using LibraryManagementDomain.DTO;
using System.Text.Json;

namespace WorkerService
{
    public class BookWorker : BackgroundService
    {
        private readonly IRabbitMQReceiver<BookDTO> _rabbitMQReceiver;
        private readonly ILogger<BookWorker> _logger;

        public BookWorker(IRabbitMQReceiver<BookDTO> rabbitMQReceiver, ILogger<BookWorker> logger)
        {
            _rabbitMQReceiver = rabbitMQReceiver;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BookWorker is starting and listening for RabbitMQ messages...");

            // Start listening to the RabbitMQ queue
            _rabbitMQReceiver.Receive("bookQueue", async message =>
            {
                _logger.LogInformation($"Received Message: {message}");

                // Deserialize the message (if JSON payload is used)
                try
                {
                    // Log relevant fields from the deserialized BookDTO
                    _logger.LogInformation($"Deserialized BookDTO: Title: {message.Title}, AuthorId: {message.AuthorId}");
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
