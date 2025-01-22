using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernal;

public interface IRabbitMQReceiver<T>
{
    /// <summary>
    /// Starts receiving messages from a RabbitMQ queue.
    /// </summary>
    /// <param name="queueName">The name of the RabbitMQ queue.</param>
    /// <param name="onMessageReceived">Callback function invoked when a message is received.</param>
    void Receive(string queueName, Func<T, Task> onMessageReceived);
}
