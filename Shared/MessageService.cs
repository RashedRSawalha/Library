//using System.Threading.Tasks;

//public class MessageService
//{
//    private readonly RabbitMQSender<string> _sender;
//    private readonly RabbitMQReceiver<string> _receiver;

//    public MessageService(RabbitMQSender<string> sender, RabbitMQReceiver<string> receiver)
//    {
//        _sender = sender;
//        _receiver = receiver;
//    }

//    public void SendHelloWorld()
//    {
//        _sender.SendMessage(
//            message: "Hello, World!",
//            exchangeName: "hello_exchange",
//            routingKey: "hello_routing",
//            queueName: "hello_queue"
//        );
//    }

//    public void StartListening()
//    {
//        // Use the Receive method instead of StartListening
//        _receiver.Receive("hello_queue", async message =>
//        {
//            Console.WriteLine($"Received message: {message}");
//            await Task.CompletedTask; // Process the message
//        });
//    }
//}
