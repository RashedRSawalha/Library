using Shared.Contracts;
namespace Shared.Helpers
{
    public class Dispatcher
    {
        private readonly IServiceProvider _provider;

        public Dispatcher(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = _provider.GetService(handlerType);

            if (handler == null)
            {
                throw new InvalidOperationException($"No handler registered for {typeof(TCommand).Name}");
            }

            await handler.Handle((dynamic)command, cancellationToken);
        }

        public async Task<T> DispatchAsync<T>(IQuery<T> query, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(T));
            dynamic handler = _provider.GetService(handlerType);

            if (handler == null)
            {
                throw new InvalidOperationException($"No handler registered for {handlerType.Name}");
            }

            return await handler.Handle((dynamic)query, cancellationToken);
        }
    }
}