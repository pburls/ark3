namespace Ark3.Command
{
    public interface ICommandHandlerFactory<in TCommand> where TCommand : ICommand
    {
        ICommandHandler<TCommand> CreateHandler();
    }
}
