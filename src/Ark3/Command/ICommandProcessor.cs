namespace Ark3.Command
{
    public interface ICommandProcessor
    {
        object Execute(ICommand command);
        void RegisterHandlerFactory<TCommand>(ICommandHandlerFactory<TCommand> factory) where TCommand : ICommand;
    }
}