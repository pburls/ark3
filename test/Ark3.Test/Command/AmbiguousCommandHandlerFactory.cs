using Ark3.Command;

namespace Ark3.Test.Command
{
    class AmbiguousCommandHandlerFactory :
        ICommandHandlerFactory<CommandA>,
        ICommandHandlerFactory<CommandB>
    {
        readonly AmbiguousCommandHandler _commandHandler;

        public AmbiguousCommandHandlerFactory(ICommandHandler<CommandA> commandHandlerA, ICommandHandler<CommandB> commandHandlerB)
        {
            _commandHandler = new AmbiguousCommandHandler(commandHandlerA, commandHandlerB);
        }

        ICommandHandler<CommandA> ICommandHandlerFactory<CommandA>.CreateHandler()
        {
            return _commandHandler;
        }

        ICommandHandler<CommandB> ICommandHandlerFactory<CommandB>.CreateHandler()
        {
            return _commandHandler;
        }
    }
}
