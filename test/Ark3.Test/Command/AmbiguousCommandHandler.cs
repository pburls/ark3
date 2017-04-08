using Ark3.Command;

namespace Ark3.Test.Command
{
    class AmbiguousCommandHandler :
        ICommandHandler<CommandA>,
        ICommandHandler<CommandB>
    {
        readonly ICommandHandler<CommandA> _commandHandlerA;
        readonly ICommandHandler<CommandB> _commandHandlerB;

        public AmbiguousCommandHandler(ICommandHandler<CommandA> commandHandlerA, ICommandHandler<CommandB> commandHandlerB)
        {
            _commandHandlerA = commandHandlerA;
            _commandHandlerB = commandHandlerB;
        }

        public void Execute(CommandA command)
        {
            _commandHandlerA.Execute(command);
        }

        public void Execute(CommandB command)
        {
            _commandHandlerB.Execute(command);
        }
    }
}
