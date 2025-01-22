using Kernal;
using RabbitMQ.Client;
using WorkerService;



var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    // Register the Worker as a hosted service
    services.AddHostedService<AuthorWorker>();
    services.AddHostedService<BookWorker>();
    services.AddHostedService<CourseWorker>();


    // Register RabbitMQ Connection Factory
    services.AddSingleton(sp =>
    {
        return new ConnectionFactory
        {
            HostName = "localhost", // RabbitMQ Host
            UserName = "guest",     // RabbitMQ Username
            Password = "guest"      // RabbitMQ Password
        };
    });

    // Register RabbitMQ Receiver using the shared project
    services.AddSingleton(typeof(IRabbitMQReceiver<>), typeof(RabbitMQReceiver<>));
});

var host = builder.Build();
await host.RunAsync();
