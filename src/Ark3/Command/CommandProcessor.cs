using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ark3.Command
{
    public class CommandProcessor : ICommandProcessor
    {
        readonly Dictionary<Type, object> _commandHandlerFactories;
        readonly TypeInfo _commandHandlerFactoryGenericType = typeof(ICommandHandlerFactory<>).GetTypeInfo();

        public CommandProcessor()
        {
            _commandHandlerFactories = new Dictionary<Type, object>();
        }

        public void RegisterHandlerFactory<TCommand>(ICommandHandlerFactory<TCommand> factory) where TCommand : ICommand
        {
            Type commandType = typeof(TCommand);

            _commandHandlerFactories.Add(commandType, factory);
        }

        public object Execute(ICommand command)
        {
            Type commandType = command.GetType();
            object commandHandlerFactory;
            object commandHandler = null;

            if (_commandHandlerFactories.TryGetValue(commandType, out commandHandlerFactory))
            {
                TypeInfo commandHandlerFactoryType = _commandHandlerFactoryGenericType.MakeGenericType(commandType).GetTypeInfo();

                MethodInfo createMethod = commandHandlerFactoryType.GetDeclaredMethod("CreateHandler");
                commandHandler = createMethod.Invoke(commandHandlerFactory, null);

                if (commandHandler != null)
                {
                    MethodInfo executeMethod = commandHandler.GetType().GetTypeInfo().GetDeclaredMethod("Execute");
                    executeMethod.Invoke(commandHandler, new[] { command });
                }
            }

            return commandHandler;
        }
    }
}
