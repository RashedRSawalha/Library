using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Kernal;

public class RabbitMQReceiver<T> : IRabbitMQReceiver<T>, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQReceiver(ConnectionFactory factory)
    {
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Receive(string queueName, Func<T, Task> onMessageReceived)
    {
        // Ensure the queue exists
        _channel.QueueDeclare(queue: queueName,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageString = Encoding.UTF8.GetString(body);

            try
            {
                // Deserialize the message into type T
                var message = JsonSerializer.Deserialize<T>(messageString);

                if (message != null)
                {
                    // Invoke the provided callback function with the message
                    await onMessageReceived(message);
                }
                else
                {
                    Console.WriteLine("Received null or invalid message.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        Console.WriteLine($"Listening on queue: {queueName}");
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}
