using Kernal;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;


namespace Kernal;

public class RabbitMQSender<T> : IRabbitMQSender<T>, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQSender(string hostName = "localhost", string userName = "guest", string password = "guest")
    {
        _connection = new ConnectionFactory()
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        }.CreateConnection();

        _channel = _connection.CreateModel();

    }

    public void SendMessage<T>(T message, string exchangeName, string routingKey = "", string queueName = null)
    {
        if (!string.IsNullOrEmpty(queueName))
        {
            // Declare the queue to ensure it exists
            _channel.QueueDeclare(queue: queueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

        }
        // Bind the queue to the exchange
        _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);

        // Serialize the message
        var messageBody = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(messageBody);

        // Make the message persistent
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        // Publish the message
        _channel.BasicPublish(exchange: exchangeName,
                              routingKey: routingKey,
                              basicProperties: properties,
                              body: body);

        Console.WriteLine($" [x] Sent {messageBody}");
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }


}
