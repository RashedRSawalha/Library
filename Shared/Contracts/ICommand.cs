using System.Threading;
using System.Threading.Tasks;



namespace Shared.Contracts
{
    public interface ICommand { }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task Handle(TCommand command, CancellationToken cancellationToken= default);
    }
}