using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernal;

public interface IRabbitMQSender<T>
{
    /// <summary>
    /// Sends a message to the specified RabbitMQ exchange.
    /// </summary>
    /// <typeparam name="T">Type of the message to be sent.</typeparam>
    /// <param name="message">The message to send.</param>
    /// <param name="exchangeName">The exchange name.</param>
    /// <param name="routingKey">The routing key (optional).</param>
    /// <param name="queueName">The queue name to declare (optional).</param>
    void SendMessage<T>(T message, string exchangeName, string routingKey = "", string queueName = null);
}
