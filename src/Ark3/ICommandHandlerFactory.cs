namespace Ark3
{
    public interface ICommandHandlerFactory<in TCommand> where TCommand : ICommand
    {
        ICommandHandler<TCommand> CreateHandler();
    }
}
